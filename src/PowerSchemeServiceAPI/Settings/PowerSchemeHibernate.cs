using System;
using PowerManagerAPI;

namespace PowerSchemeServiceAPI.Settings;

public class PowerSchemeHibernate(
    Guid powerSchemeGuid,
    PowerSchemeDcAcValues dcAcValues) :
    BaseStatePowerSchemeValues(powerSchemeGuid)
{
    protected override PowerSchemeValues State => new(
            Setting.HIBERNATEIDLE,
            dcAcValues.DcSettings,
            dcAcValues.AcSettings);

    protected override SettingSubgroup SettingSubgroup =>
        SettingSubgroup.SLEEP_SUBGROUP;
}
