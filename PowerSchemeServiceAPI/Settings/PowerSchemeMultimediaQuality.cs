using PowerManagerAPI;
using System;

namespace PowerSchemeServiceAPI.Settings
{
    public class PowerSchemeMultimediaQuality : BaseStatePowerSchemeValues
    {
        private readonly PowerSchemeDCACValues _DCACValues;

        public PowerSchemeMultimediaQuality(Guid powerSchemeGuid, PowerSchemeDCACValues DCACValues) : base(powerSchemeGuid)
        {
            _DCACValues = DCACValues;
        }
        protected override SettingSubgroup SettingSubgroup
            => SettingSubgroup.MULTIMEDIA_SUBGROUP;

        protected override PowerSchemeValues State
            => new PowerSchemeValues(
                    Setting.MULTQUALITY,
                    _DCACValues.DCSettings,
                    _DCACValues.ACSettings);
    }
}
