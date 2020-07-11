﻿using System;
using PowerManagerAPI;
using PowerScheme.Services;

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

        public override PowerSchemeValues MinState
            => new PowerSchemeValues(Setting.PROCTHROTTLEMIN, _minState.DCSettings, _minState.ACSettings);  // 5/80

        public override PowerSchemeValues MaxState
            => new PowerSchemeValues(Setting.PROCTHROTTLEMAX, _maxState.DCSettings, _maxState.ACSettings); // 60/80

        public override SettingSubgroup SettingSubgroup
            => SettingSubgroup.PROCESSOR_SETTINGS_SUBGROUP;
    }
}