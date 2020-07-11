using PowerManagerAPI;
using System;

namespace PowerScheme.Settings
{
    public class PowerSchemeTurnOffDisplay : BaseStatePowerSchemeValues
    {
        private readonly PowerSchemeDCACValues _DCACValues;

        public PowerSchemeTurnOffDisplay(Guid powerSchemeGuid, PowerSchemeDCACValues DCACValues) : base(powerSchemeGuid)
        {
            _DCACValues = DCACValues;
        }

        public override SettingSubgroup SettingSubgroup
            => SettingSubgroup.VIDEO_SUBGROUP;

        public override PowerSchemeValues State
            => new PowerSchemeValues(Setting.VIDEOIDLE, _DCACValues.DCSettings, _DCACValues.ACSettings);
    }
}
