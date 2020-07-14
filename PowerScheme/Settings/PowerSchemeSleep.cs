using PowerManagerAPI;
using System;

namespace PowerScheme.Settings
{
    public class PowerSchemeSleep: BaseStatePowerSchemeValues
    {
        private readonly PowerSchemeDCACValues _DCACValues;

        public PowerSchemeSleep(Guid powerSchemeGuid, PowerSchemeDCACValues DCACValues) : base(powerSchemeGuid)
        {
            _DCACValues = DCACValues;
        }

        protected override PowerSchemeValues State 
            => new PowerSchemeValues(
                    Setting.STANDBYIDLE, 
                    _DCACValues.DCSettings, 
                    _DCACValues.ACSettings);

        protected override SettingSubgroup SettingSubgroup
            => SettingSubgroup.SLEEP_SUBGROUP;
    }
}
