using System;
using PowerManagerAPI;

namespace PowerSchemeServiceAPI.Settings;

public class PowerSchemeProcessorThrottle(
    Guid guid,
    PowerSchemeDcAcValues minState,
    PowerSchemeDcAcValues maxState) :
    BaseMinMaxPowerSchemeValues(guid)
{
    protected override PowerSchemeValues MinState =>
        new(Setting.PROCTHROTTLEMIN, minState.DcSettings, minState.AcSettings);

    protected override PowerSchemeValues MaxState =>
        new(Setting.PROCTHROTTLEMAX, maxState.DcSettings, maxState.AcSettings);

    protected override SettingSubgroup SettingSubgroup =>
        SettingSubgroup.PROCESSOR_SETTINGS_SUBGROUP;
}
