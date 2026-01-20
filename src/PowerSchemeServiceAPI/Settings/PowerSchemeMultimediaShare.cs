using System;
using PowerManagerAPI;

namespace PowerSchemeServiceAPI.Settings;

public class PowerSchemeMultimediaShare(
    Guid powerSchemeGuid,
    PowerSchemeDcAcValues dcAcValues) :
    BaseStatePowerSchemeValues(powerSchemeGuid)
{
    protected override SettingSubgroup SettingSubgroup =>
        SettingSubgroup.MULTIMEDIA_SUBGROUP;

    protected override PowerSchemeValues State => new(
            Setting.MULTSHARE,
            dcAcValues.DcSettings,
            dcAcValues.AcSettings);
}
