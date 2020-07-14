﻿using PowerManagerAPI;
using System;

namespace PowerScheme.Settings
{
    public class PowerSchemeProcessorThrottle : BaseMinMaxPowerSchemeValues
    {
        private readonly PowerSchemeDCACValues _minState;
        private readonly PowerSchemeDCACValues _maxState;

        public PowerSchemeProcessorThrottle(Guid guid, PowerSchemeDCACValues minState, PowerSchemeDCACValues maxState) :
            base(guid)
        {
            _minState = minState;
            _maxState = maxState;
        }

        protected override PowerSchemeValues MinState
            => new PowerSchemeValues(Setting.PROCTHROTTLEMIN, _minState.DCSettings, _minState.ACSettings);  

        protected override PowerSchemeValues MaxState
            => new PowerSchemeValues(Setting.PROCTHROTTLEMAX, _maxState.DCSettings, _maxState.ACSettings); 

        protected override SettingSubgroup SettingSubgroup
            => SettingSubgroup.PROCESSOR_SETTINGS_SUBGROUP;
    }
}