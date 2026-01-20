using System;
using PowerManagerAPI;

namespace PowerSchemeServiceAPI.Settings;

public class PowerSchemeSleep(
    Guid powerSchemeGuid,
    PowerSchemeDcAcValues dcAcValues) :
    BaseStatePowerSchemeValues(powerSchemeGuid)
{
    protected override PowerSchemeValues State => new(
            Setting.STANDBYIDLE,
            dcAcValues.DcSettings,
            dcAcValues.AcSettings);

    protected override SettingSubgroup SettingSubgroup =>
        SettingSubgroup.SLEEP_SUBGROUP;
}
