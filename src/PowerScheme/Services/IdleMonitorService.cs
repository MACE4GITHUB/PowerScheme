using System;
using System.Linq;
using System.Threading.Tasks;
using PowerScheme.Addins.IdleMonitoring;
using PowerScheme.Services.Settings;
using PowerSchemeServiceAPI;
using PowerSchemeServiceAPI.EventsArgs;

namespace PowerScheme.Services;

public sealed class IdleMonitorService(
    IIdleMonitor idleMonitor,
    IPowerSchemeService power,
    IIdleSettingsStore settings) :
    IIdleMonitorService
{
    private readonly object _lifecycleSync = new();
    private Task _startTask = Task.CompletedTask;
    private Task? _restartTask;
    private long _generation;
    private long _restartGeneration;
    private bool _started;
    private bool _disposed;
    private bool _restorePending;
    private Guid _restoreSchemeId;
    private Guid _mainSettingAtIdle;
    private bool _switchingScheme;
    private EventHandler<IdleMonitorFaultEventArgs>? _faultHandler;

    public void Start()
    {
        lock (_lifecycleSync)
        {
            ThrowIfDisposed();
            if (_started)
            {
                return;
            }

            if (!_startTask.IsCompleted || _restartTask is { IsCompleted: false } || !idleMonitor.WaitForStopAsync().IsCompleted)
            {
                throw new InvalidOperationException("The previous monitoring cycle has not stopped. Use RestartAsync instead.");
            }

            _generation++;
            StartCore();
        }
    }

    private Task StartCore()
    {
        power.ActivePowerSchemeChanged += ActivePowerSchemeChanged;
        idleMonitor.OnIdle += OnIdle;
        idleMonitor.OnActive += OnActive;
        var generation = _generation;
        _faultHandler = (_, _) => MonitorFaulted(generation);
        idleMonitor.Faulted += _faultHandler;
        _started = true;
        try
        {
            var idleMonitorOption = settings.LoadMonitorOptions();
            if (!idleMonitorOption.IsValid)
            {
                idleMonitorOption = IdleMonitorOption.Default;
                settings.SaveMonitorOptions(idleMonitorOption);
            }

            _startTask = idleMonitor.StartAsync(idleMonitorOption);
            if (_startTask.IsCompleted)
            {
                _startTask.GetAwaiter().GetResult();
            }

            return _startTask;
        }
        catch
        {
            _startTask = Task.CompletedTask;
            StopCore();
            throw;
        }
    }

    public void Stop()
    {
        lock (_lifecycleSync)
        {
            _generation++;
            StopCore();
        }
    }

    private void StopCore()
    {
        _started = false;
        idleMonitor.OnIdle -= OnIdle;
        idleMonitor.OnActive -= OnActive;
        idleMonitor.Faulted -= _faultHandler;
        _faultHandler = null;
        power.ActivePowerSchemeChanged -= ActivePowerSchemeChanged;
        idleMonitor.Stop();
    }

    public Task RestartAsync()
    {
        lock (_lifecycleSync)
        {
            ThrowIfDisposed();
            if (!_started && _restartTask is { IsCompleted: false } && _restartGeneration == _generation)
            {
                return _restartTask;
            }

            var generation = ++_generation;
            _restartGeneration = generation;
            StopCore();
            _restartTask = RestartCoreAsync(generation, _startTask);
            return _restartTask;
        }
    }

    private async Task RestartCoreAsync(long generation, Task previousStart)
    {
        try
        {
            await previousStart;
            await idleMonitor.WaitForStopAsync();

            Task start;
            lock (_lifecycleSync)
            {
                if (_disposed || generation != _generation)
                {
                    return;
                }

                start = StartCore();
            }

            await start;
        }
        catch
        {
            lock (_lifecycleSync)
            {
                if (generation == _generation)
                {
                    _startTask = Task.CompletedTask;
                    StopCore();
                }
            }

            throw;
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(IdleMonitorService));
        }
    }

    private void MonitorFaulted(long generation)
    {
        lock (_lifecycleSync)
        {
            if (!_disposed && _started && generation == _generation)
            {
                StopCore();
            }
        }
    }

    private void ActivePowerSchemeChanged(object sender, PowerSchemeEventArgs e)
    {
        lock (_lifecycleSync)
        {
            if (_started && !_disposed)
            {
                UpdateMainScheme(e);
            }
        }
    }

    private void UpdateMainScheme(PowerSchemeEventArgs e)
    {
        if (_switchingScheme)
        {
            return;
        }

        var idlePowerSchemeGuid = settings.IdlePowerSchemeId;

        if (idlePowerSchemeGuid == Guid.Empty)
        {
            return;
        }

        var mainPowerSchemeUpdate = IdlePowerSchemePolicy.GetMainPowerSchemeUpdate(
            e.PreviousPowerScheme.Guid,
            e.ActivePowerScheme.Guid,
            idlePowerSchemeGuid,
            settings.MainPowerSchemeId);

        if (mainPowerSchemeUpdate is { } mainPowerSchemeId)
        {
            settings.MainPowerSchemeId = mainPowerSchemeId;
            if (_restorePending)
            {
                _restoreSchemeId = mainPowerSchemeId;
                _mainSettingAtIdle = mainPowerSchemeId;
            }
        }
    }

    private void OnActive(object sender, EventArgs e)
    {
        lock (_lifecycleSync)
        {
            if (_started && !_disposed)
            {
                RestoreMainScheme(e);
            }
        }
    }

    private void RestoreMainScheme(EventArgs e)
    {
        if (e is IdleMonitorActiveEventArgs { IsInitialSample: true } && !_restorePending)
        {
            return;
        }

        var idlePowerSchemeGuid = settings.IdlePowerSchemeId;

        if (idlePowerSchemeGuid == Guid.Empty && !_restorePending)
        {
            return;
        }

        var configuredMain = settings.MainPowerSchemeId;
        if (_restorePending && configuredMain != _mainSettingAtIdle && configuredMain != Guid.Empty && configuredMain != idlePowerSchemeGuid)
        {
            _restoreSchemeId = configuredMain;
            _mainSettingAtIdle = configuredMain;
        }

        var mainPowerSchemeGuid = _restorePending ? _restoreSchemeId : configuredMain;
        var previousPowerScheme =
            power
            .PowerSchemes
            .FirstOrDefault(x => x.Guid == mainPowerSchemeGuid) ??
            power
            .PowerProfPowerSchemes
            .First();

        _restorePending = true;
        _restoreSchemeId = previousPowerScheme.Guid;
        SetActiveScheme(previousPowerScheme);
        if (configuredMain != previousPowerScheme.Guid)
        {
            settings.MainPowerSchemeId = previousPowerScheme.Guid;
        }
        _restorePending = false;
    }

    private void OnIdle(object sender, EventArgs e)
    {
        lock (_lifecycleSync)
        {
            if (_started && !_disposed)
            {
                EnterIdleScheme();
            }
        }
    }

    private void EnterIdleScheme()
    {
        var idlePowerSchemeGuid = settings.IdlePowerSchemeId;

        if (idlePowerSchemeGuid == Guid.Empty || _restorePending)
        {
            return;
        }

        var currentSchemeId = power.ActivePowerScheme.Guid;
        _mainSettingAtIdle = settings.MainPowerSchemeId;
        _restoreSchemeId = currentSchemeId == idlePowerSchemeGuid ? _mainSettingAtIdle : currentSchemeId;
        _restorePending = true;
        if (_mainSettingAtIdle != _restoreSchemeId)
        {
            settings.MainPowerSchemeId = _restoreSchemeId;
            _mainSettingAtIdle = _restoreSchemeId;
        }

        if (settings.KeepBrightness)
        {
            power.CopyBrightness(settings.MainPowerSchemeId, idlePowerSchemeGuid);
        }

        var idlePowerScheme =
            power
            .PowerSchemes
            .FirstOrDefault(x => x.Guid == idlePowerSchemeGuid) ??
            power
                .PowerProfPowerSchemes
                .Last(x => !x.IsMaxPerformance && x.IsNative);

        SetActiveScheme(idlePowerScheme);
    }

    private void SetActiveScheme(PowerSchemeServiceAPI.Model.IPowerScheme scheme)
    {
        _switchingScheme = true;
        try
        {
            power.SetActivePowerScheme(scheme);
        }
        finally
        {
            _switchingScheme = false;
        }
    }

    public void Dispose()
    {
        lock (_lifecycleSync)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _generation++;
            StopCore();
        }
    }
}
