using PowerManagerAPI;
using System;

namespace PowerSchemeServiceAPI.Settings
{
    public class PowerSchemeMultimediaPlay : BaseStatePowerSchemeValues
    {
        private readonly PowerSchemeDCACValues _DCACValues;

        public PowerSchemeMultimediaPlay(Guid powerSchemeGuid, PowerSchemeDCACValues DCACValues) : base(powerSchemeGuid)
        {
            _DCACValues = DCACValues;
        }
        protected override SettingSubgroup SettingSubgroup
            => SettingSubgroup.MULTIMEDIA_SUBGROUP;

        protected override PowerSchemeValues State
            => new PowerSchemeValues(
                    Setting.MULTPLAY,
                    _DCACValues.DCSettings,
                    _DCACValues.ACSettings);
    }
}
