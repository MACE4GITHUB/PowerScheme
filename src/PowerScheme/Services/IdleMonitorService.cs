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

    private void ActivePowerSchemeChanged(object sender, PowerSchemeEventArgs e)
    {
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
        }
    }

    private void OnActive(object sender, EventArgs e)
    {
        var idlePowerSchemeGuid = settings.IdlePowerSchemeId;

        if (idlePowerSchemeGuid == Guid.Empty)
        {
            return;
        }

        var mainPowerSchemeGuid = settings.MainPowerSchemeId;
        var previousPowerScheme =
            power
            .PowerSchemes
            .FirstOrDefault(x => x.Guid == mainPowerSchemeGuid) ??
            power
            .PowerProfPowerSchemes
            .First();

        power.SetActivePowerScheme(previousPowerScheme);
    }

    private void OnIdle(object sender, EventArgs e)
    {
        var idlePowerSchemeGuid = settings.IdlePowerSchemeId;

        if (idlePowerSchemeGuid == Guid.Empty)
        {
            return;
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

        power.SetActivePowerScheme(idlePowerScheme);
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
