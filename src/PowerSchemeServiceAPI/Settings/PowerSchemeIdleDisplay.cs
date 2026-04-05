using System;
using PowerManagerAPI;

namespace PowerSchemeServiceAPI.Settings;

public class PowerSchemeIdleDisplay(
    Guid powerSchemeGuid,
    PowerSchemeDcAcValues dcAcValues) :
    BaseStatePowerSchemeValues(powerSchemeGuid)
{
    protected override PowerSchemeValues State => new(
            Setting.VIDEOIDLE,
            dcAcValues.DcSettings,
            dcAcValues.AcSettings);

    protected override SettingSubgroup SettingSubgroup =>
        SettingSubgroup.VIDEO_SUBGROUP;
}
