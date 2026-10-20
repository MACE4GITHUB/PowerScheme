using System;
using PowerScheme.Addins.IdleMonitoring;

namespace PowerScheme.Services.Settings;

public interface IIdleSettingsStore
{
    IdleMonitorOption LoadMonitorOptions();
    void SaveMonitorOptions(IdleMonitorOption options);
    Guid IdlePowerSchemeId { get; }
    Guid MainPowerSchemeId { get; set; }
    bool KeepBrightness { get; }
}
