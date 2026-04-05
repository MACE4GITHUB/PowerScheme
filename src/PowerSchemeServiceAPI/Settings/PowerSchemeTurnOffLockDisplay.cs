using System;
using PowerManagerAPI;

namespace PowerSchemeServiceAPI.Settings;

public class PowerSchemeTurnOffLockDisplay(
    Guid powerSchemeGuid,
    PowerSchemeDcAcValues dcAcValues) :
    BaseStatePowerSchemeValues(powerSchemeGuid)
{
    protected override SettingSubgroup SettingSubgroup
        => SettingSubgroup.VIDEO_SUBGROUP;

    protected override PowerSchemeValues State =>
        new(Setting.VIDEOCONLOCK, dcAcValues.DcSettings, dcAcValues.AcSettings);
}
