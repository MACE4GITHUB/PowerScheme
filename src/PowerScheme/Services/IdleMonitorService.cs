using System;
using System.Linq;
using System.Threading;
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
    public void Start()
    {
        power.ActivePowerSchemeChanged += ActivePowerSchemeChanged;
        idleMonitor.OnIdle += OnIdle;
        idleMonitor.OnActive += OnActive;
        var idleMonitorOption = settings.LoadMonitorOptions();

        if (!idleMonitorOption.IsValid)
        {
            idleMonitorOption = IdleMonitorOption.Default;

            settings.SaveMonitorOptions(idleMonitorOption);
        }

        idleMonitor.StartAsync(idleMonitorOption);
    }

    public void Stop()
    {
        idleMonitor.OnIdle -= OnIdle;
        idleMonitor.OnActive -= OnActive;
        power.ActivePowerSchemeChanged -= ActivePowerSchemeChanged;
        idleMonitor.Stop();
    }

    public void Restart()
    {
        Stop();
        Thread.Sleep(1000);
        Start();
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
        Stop();
    }
}
