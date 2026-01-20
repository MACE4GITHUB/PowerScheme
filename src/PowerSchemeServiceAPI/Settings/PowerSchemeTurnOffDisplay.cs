using System;
using PowerManagerAPI;

namespace PowerSchemeServiceAPI.Settings;

public class PowerSchemeTurnOffDisplay(
    Guid powerSchemeGuid,
    PowerSchemeDcAcValues dcAcValues) :
    BaseStatePowerSchemeValues(powerSchemeGuid)
{
    protected override SettingSubgroup SettingSubgroup
        => SettingSubgroup.VIDEO_SUBGROUP;

    protected override PowerSchemeValues State =>
        new(Setting.VIDEOIDLE, dcAcValues.DcSettings, dcAcValues.AcSettings);
}
