using System;
using PowerManagerAPI;

namespace PowerSchemeServiceAPI.Settings;

/// <summary>
/// <para>Do nothing - 0</para>
/// <para>Sleep - 1</para>
/// <para>Hibernate - 2</para>
/// <para>Shut down - 3</para>
/// </summary>
/// <param name="powerSchemeGuid"></param>
/// <param name="dcAcValues"></param>
public class PowerSchemeLid(
    Guid powerSchemeGuid,
    PowerSchemeDcAcValues dcAcValues) :
    BaseStatePowerSchemeValues(powerSchemeGuid)
{
    protected override PowerSchemeValues State => new(
            Setting.LIDACTION,
            dcAcValues.DcSettings,
            dcAcValues.AcSettings);

    protected override SettingSubgroup SettingSubgroup =>
        SettingSubgroup.SYSTEM_BUTTON_SUBGROUP;
}
