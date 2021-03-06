﻿using PowerManagerAPI;
using System;

namespace PowerSchemeServiceAPI.Settings
{
    public class PowerSchemeMultimediaShare : BaseStatePowerSchemeValues
    {
        private readonly PowerSchemeDCACValues _DCACValues;

        public PowerSchemeMultimediaShare(Guid powerSchemeGuid, PowerSchemeDCACValues DCACValues) : base(powerSchemeGuid)
        {
            _DCACValues = DCACValues;
        }
        protected override SettingSubgroup SettingSubgroup
            => SettingSubgroup.MULTIMEDIA_SUBGROUP;

        protected override PowerSchemeValues State
            => new PowerSchemeValues(
                    Setting.MULTSHARE,
                    _DCACValues.DCSettings,
                    _DCACValues.ACSettings);
    }
}
