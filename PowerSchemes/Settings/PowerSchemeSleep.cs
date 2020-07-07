﻿using PowerManagerAPI;
using System;

namespace PowerSchemes.Settings
{
    public class PowerSchemeSleep: BaseStatePowerSchemeValues
    {
        private PowerSchemeDCACValues _DCACValues;

        public PowerSchemeSleep(Guid powerSchemeGuid, PowerSchemeDCACValues DCACValues) : base(powerSchemeGuid)
        {
            _DCACValues = DCACValues;
        }

        public override PowerSchemeValues State 
            => new PowerSchemeValues(
                    Setting.STANDBYIDLE, 
                    _DCACValues.DCSettings, 
                    _DCACValues.ACSettings);
           
        public override SettingSubgroup SettingSubgroup
            => SettingSubgroup.SLEEP_SUBGROUP;
    }
}
