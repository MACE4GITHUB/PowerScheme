using System;
using PowerManagerAPI;

namespace PowerSchemeServiceAPI.Settings;

public class PowerSchemeMultimediaPlay(
    Guid powerSchemeGuid,
    PowerSchemeDcAcValues dcAcValues) :
    BaseStatePowerSchemeValues(powerSchemeGuid)
{
    protected override SettingSubgroup SettingSubgroup =>
        SettingSubgroup.MULTIMEDIA_SUBGROUP;

    protected override PowerSchemeValues State => new(
            Setting.MULTPLAY,
            dcAcValues.DcSettings,
            dcAcValues.AcSettings);
}
