using PowerManagerAPI;
using System;

namespace PowerScheme.Settings
{
    public class PowerSchemeLid: BaseStatePowerSchemeValues
    {
        private readonly PowerSchemeDCACValues _DCACValues;

        public PowerSchemeLid(Guid powerSchemeGuid, PowerSchemeDCACValues DCACValues) : base(powerSchemeGuid)
        {
            _DCACValues = DCACValues;
        }

        public override PowerSchemeValues State 
            => new PowerSchemeValues(
                    Setting.LIDACTION, 
                    _DCACValues.DCSettings, 
                    _DCACValues.ACSettings);
           
        public override SettingSubgroup SettingSubgroup
            => SettingSubgroup.SYSTEM_BUTTON_SUBGROUP;
    }
}
