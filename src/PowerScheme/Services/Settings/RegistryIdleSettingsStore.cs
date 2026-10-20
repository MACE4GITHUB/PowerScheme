using System;
using PowerScheme.Addins.IdleMonitoring;
using RegistryManager;

namespace PowerScheme.Services.Settings;

internal sealed class RegistryIdleSettingsStore(string company, string product) : IIdleSettingsStore
{
    public IdleMonitorOption LoadMonitorOptions() =>
        new(
            RegistryService.GetIdleThresholdInMilliseconds(company, product),
            RegistryService.GetPollingActiveTimeInMilliseconds(company, product),
            RegistryService.GetPollingIdleTimeInMilliseconds(company, product));

    public void SaveMonitorOptions(IdleMonitorOption options)
    {
        RegistryService.SetIdleThresholdInMilliseconds(company, product, options.IdleThresholdInMilliseconds);
        RegistryService.SetPollingActiveTimeInMilliseconds(company, product, options.PollingActiveTimeInMilliseconds);
        RegistryService.SetPollingIdleTimeInMilliseconds(company, product, options.PollingIdleTimeInMilliseconds);
    }

    public Guid IdlePowerSchemeId => RegistryService.GetIdleMonitoring(company, product);

    public Guid MainPowerSchemeId
    {
        get => RegistryService.GetMainMonitoring(company, product);
        set => RegistryService.SetMainPowerScheme(company, product, value);
    }

    public bool KeepBrightness => RegistryService.GetKeepBrightness(company, product);
}
