using System;
using PowerManagerAPI;

namespace PowerSchemeServiceAPI.Settings;

public class PowerSchemeLid: BaseStatePowerSchemeValues
{
    private readonly PowerSchemeDCACValues _DCACValues;

    /// <summary>
    /// <para>Do nothing - 0</para>
    /// <para>Sleep - 1</para>
    /// <para>Hibernate - 2</para>
    /// <para>Shut down - 3</para> 
    /// </summary>
    /// <param name="powerSchemeGuid"></param>
    /// <param name="DCACValues"></param>
    public PowerSchemeLid(Guid powerSchemeGuid, PowerSchemeDCACValues DCACValues) : base(powerSchemeGuid)
    {
        _DCACValues = DCACValues;
    }

    protected override PowerSchemeValues State 
        => new(
            Setting.LIDACTION, 
            _DCACValues.DCSettings, 
            _DCACValues.ACSettings);

    protected override SettingSubgroup SettingSubgroup
        => SettingSubgroup.SYSTEM_BUTTON_SUBGROUP;
}