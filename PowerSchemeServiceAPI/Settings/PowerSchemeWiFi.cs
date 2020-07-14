using System;
using PowerManagerAPI;

namespace PowerSchemeServiceAPI.Settings
{
    public class PowerSchemeWiFi : BaseStatePowerSchemeValues
    {
        private readonly PowerSchemeDCACValues _DCACValues;

        /// <summary>
        /// <para>Maximum performance - 0</para>
        /// <para>Minimum power saving - 1</para>
        /// <para>Average energy saving - 2</para>
        /// <para>Maximum power saving - 3</para>
        /// </summary>
        /// <param name="powerSchemeGuid"></param>
        /// <param name="DCACValues"></param>
        public PowerSchemeWiFi(Guid powerSchemeGuid, PowerSchemeDCACValues DCACValues) : base(powerSchemeGuid)
        {
            _DCACValues = DCACValues;
        }

        protected override SettingSubgroup SettingSubgroup
            => SettingSubgroup.WIFI_SUBGROUP;

        protected override PowerSchemeValues State
            => new PowerSchemeValues(Setting.WIFISAVER, _DCACValues.DCSettings, _DCACValues.ACSettings);
    }
}
