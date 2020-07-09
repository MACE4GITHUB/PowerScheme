using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerManagerAPI;

namespace PowerScheme.Settings
{
    public class PowerSchemeTurnOffDisplay : BaseStatePowerSchemeValues
    {
        private PowerSchemeDCACValues _DCACValues;

        public PowerSchemeTurnOffDisplay(Guid powerSchemeGuid, PowerSchemeDCACValues DCACValues) : base(powerSchemeGuid)
        {
            _DCACValues = DCACValues;
        }

        public override SettingSubgroup SettingSubgroup
            => SettingSubgroup.VIDEO_SUBGROUP;

        public override PowerSchemeValues State
            => new PowerSchemeValues(Setting.VIDEOIDLE, _DCACValues.DCSettings, _DCACValues.ACSettings); // 300/7200
    }
}
