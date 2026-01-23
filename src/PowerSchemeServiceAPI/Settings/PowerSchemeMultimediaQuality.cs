using System;
using PowerManagerAPI;

namespace PowerSchemeServiceAPI.Settings;

public class PowerSchemeMultimediaQuality(
    Guid powerSchemeGuid,
    PowerSchemeDcAcValues dcAcValues) :
    BaseStatePowerSchemeValues(powerSchemeGuid)
{
    protected override SettingSubgroup SettingSubgroup =>
        SettingSubgroup.MULTIMEDIA_SUBGROUP;

    protected override PowerSchemeValues State => new(
            Setting.MULTQUALITY,
            dcAcValues.DcSettings,
            dcAcValues.AcSettings);
}
