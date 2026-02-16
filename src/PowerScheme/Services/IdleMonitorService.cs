using System;
using System.Linq;
using PowerScheme.Addins.IdleMonitoring;
using PowerScheme.Model;
using PowerSchemeServiceAPI;
using PowerSchemeServiceAPI.EventsArgs;
using RegistryManager;

namespace PowerScheme.Services;

public sealed class IdleMonitorService(
    IIdleMonitor idleMonitor,
    IPowerSchemeService power) :
    IIdleMonitorService
{
    public void Start()
    {
        power.ActivePowerSchemeChanged += ActivePowerSchemeChanged;
        idleMonitor.OnIdle += OnIdle;
        idleMonitor.OnActive += OnActive;
        var idleMonitorOption = new IdleMonitorOption(
            RegistryService.GetIdleThresholdInMilliseconds(AppInfo.CompanyName, AppInfo.ProductName),
            RegistryService.GetPollingActiveTimeInMilliseconds(AppInfo.CompanyName, AppInfo.ProductName),
            RegistryService.GetPollingIdleTimeInMilliseconds(AppInfo.CompanyName, AppInfo.ProductName));

        if (!idleMonitorOption.IsValid)
        {
            idleMonitorOption = IdleMonitorOption.Default;

            RegistryService.SetIdleThresholdInMilliseconds(AppInfo.CompanyName, AppInfo.ProductName, idleMonitorOption.IdleThresholdInMilliseconds);
            RegistryService.SetPollingActiveTimeInMilliseconds(AppInfo.CompanyName, AppInfo.ProductName, idleMonitorOption.PollingActiveTimeInMilliseconds);
            RegistryService.SetPollingIdleTimeInMilliseconds(AppInfo.CompanyName, AppInfo.ProductName, idleMonitorOption.PollingIdleTimeInMilliseconds);
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

    private static void ActivePowerSchemeChanged(object sender, PowerSchemeEventArgs e)
    {
        var idlePowerSchemeGuid = GetIdlePowerSchemeGuid();

        if (idlePowerSchemeGuid == Guid.Empty)
        {
            return;
        }

        var mainPowerSchemeGuid = GetMainPowerSchemeGuid();

        var isIdleToIdle =
            idlePowerSchemeGuid == e.PreviousPowerScheme.Guid &&
            idlePowerSchemeGuid == e.ActivePowerScheme.Guid &&
            mainPowerSchemeGuid != e.ActivePowerScheme.Guid;

        if (isIdleToIdle)
        {
            SetMainPowerScheme(e.ActivePowerScheme.Guid);
            return;
        }

        var isIdleToNotIdle =
            idlePowerSchemeGuid == e.PreviousPowerScheme.Guid &&
            idlePowerSchemeGuid != e.ActivePowerScheme.Guid &&
            mainPowerSchemeGuid != e.ActivePowerScheme.Guid;

        if (isIdleToNotIdle)
        {
            SetMainPowerScheme(e.ActivePowerScheme.Guid);
            return;
        }

        var isNotIdleToNotIdle =
            idlePowerSchemeGuid != e.PreviousPowerScheme.Guid &&
            idlePowerSchemeGuid != e.ActivePowerScheme.Guid &&
            mainPowerSchemeGuid != e.ActivePowerScheme.Guid;

        if (isNotIdleToNotIdle)
        {
            SetMainPowerScheme(e.ActivePowerScheme.Guid);
            return;
        }

        var isNotIdleToIdle =
            idlePowerSchemeGuid != e.PreviousPowerScheme.Guid &&
            idlePowerSchemeGuid == e.ActivePowerScheme.Guid &&
            mainPowerSchemeGuid != e.PreviousPowerScheme.Guid;

        if (isNotIdleToIdle)
        {
            SetMainPowerScheme(e.PreviousPowerScheme.Guid);
        }
    }

    private void OnActive(object sender, EventArgs e)
    {
        var idlePowerSchemeGuid = GetIdlePowerSchemeGuid();

        if (idlePowerSchemeGuid == Guid.Empty)
        {
            return;
        }

        var mainPowerSchemeGuid = GetMainPowerSchemeGuid();
        var previousPowerScheme =
            power
            .PowerSchemes
            .FirstOrDefault(x => x.Guid == mainPowerSchemeGuid) ??
            power
            .DefaultPowerSchemes
            .First();

        power.SetActivePowerScheme(previousPowerScheme);
    }

    private void OnIdle(object sender, EventArgs e)
    {
        var idlePowerSchemeGuid = GetIdlePowerSchemeGuid();

        if (idlePowerSchemeGuid == Guid.Empty)
        {
            return;
        }

        var idlePowerScheme =
            power
            .PowerSchemes
            .FirstOrDefault(x => x.Guid == idlePowerSchemeGuid) ??
            power
                .DefaultPowerSchemes
                .Last(x => !x.IsMaxPerformance && x.IsNative);

        power.SetActivePowerScheme(idlePowerScheme);
    }

    private static Guid GetIdlePowerSchemeGuid() =>
        RegistryService.GetIdleMonitoring(AppInfo.CompanyName, AppInfo.ProductName);

    private static Guid GetMainPowerSchemeGuid() =>
        RegistryService.GetMainMonitoring(AppInfo.CompanyName, AppInfo.ProductName);

    private static void SetMainPowerScheme(Guid guid) =>
        RegistryService.SetMainPowerScheme(AppInfo.CompanyName, AppInfo.ProductName, guid);

    public void Dispose()
    {
        Stop();
    }
}
