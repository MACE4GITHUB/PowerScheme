using System;
using PowerManagerAPI;

namespace PowerSchemeServiceAPI.Settings;

/// <summary>
/// <para>Maximum performance - 0</para>
/// <para>Minimum power saving - 1</para>
/// <para>Average energy saving - 2</para>
/// <para>Maximum power saving - 3</para>
/// </summary>
/// <param name="powerSchemeGuid"></param>
/// <param name="dcAcValues"></param>
public class PowerSchemeWiFi(
    Guid powerSchemeGuid,
    PowerSchemeDcAcValues dcAcValues) :
    BaseStatePowerSchemeValues(powerSchemeGuid)
{
    protected override SettingSubgroup SettingSubgroup
        => SettingSubgroup.WIFI_SUBGROUP;

    protected override PowerSchemeValues State => new(Setting.WIFISAVER, dcAcValues.DcSettings, dcAcValues.AcSettings);
}
