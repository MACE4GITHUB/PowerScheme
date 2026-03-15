using System;
using PowerManagerAPI;

namespace PowerSchemeServiceAPI.Settings;

public class PowerSchemeVideoNormalLevel(
    Guid powerSchemeGuid,
    PowerSchemeDcAcValues dcAcValues) :
    BaseStatePowerSchemeValues(powerSchemeGuid)
{
    protected override SettingSubgroup SettingSubgroup =>
        SettingSubgroup.VIDEO_SUBGROUP;

    protected override PowerSchemeValues State => new(
        Setting.VIDEONORMALLEVEL,
        dcAcValues.DcSettings,
        dcAcValues.AcSettings);
}
