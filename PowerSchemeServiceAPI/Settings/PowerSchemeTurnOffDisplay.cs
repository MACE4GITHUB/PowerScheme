using System;
using PowerManagerAPI;

namespace PowerSchemeServiceAPI.Settings
{
    public class PowerSchemeTurnOffDisplay : BaseStatePowerSchemeValues
    {
        private readonly PowerSchemeDCACValues _DCACValues;

        public PowerSchemeTurnOffDisplay(Guid powerSchemeGuid, PowerSchemeDCACValues DCACValues) : base(powerSchemeGuid)
        {
            _DCACValues = DCACValues;
        }

        protected override SettingSubgroup SettingSubgroup
            => SettingSubgroup.VIDEO_SUBGROUP;

        protected override PowerSchemeValues State
            => new PowerSchemeValues(Setting.VIDEOIDLE, _DCACValues.DCSettings, _DCACValues.ACSettings);
    }
}
