using PowerManagerAPI;
using System;

namespace PowerScheme.Settings
{
    public abstract class BaseMinMaxPowerSchemeValues : IApplicable
    {
        public BaseMinMaxPowerSchemeValues(Guid powerSchemeGuid)
        {
            PowerSchemeGuid = powerSchemeGuid;
        }

        public virtual PowerSchemeValues MinState { get; set; }

        public virtual PowerSchemeValues MaxState { get; set; }

        public virtual SettingSubgroup SettingSubgroup { get; }

        public Guid PowerSchemeGuid { get; }


        public void ApplyValues()
        {
            ApplyDCMinMaxValues();
            ApplyACMinMaxValues();
        }

        private void ApplyDCMinMaxValues()
        {
            ApplyDCValues(MinState);
            ApplyDCValues(MaxState);
        }

        private void ApplyACMinMaxValues()
        {
            ApplyACValues(MinState);
            ApplyACValues(MaxState);
        }

        private void ApplyDCValues(PowerSchemeValues powerSchemeValues)
        {
            if (PowerSchemeGuid == Guid.Empty)
                throw new ArgumentNullException(nameof(PowerSchemeGuid));

            if (powerSchemeValues.DCSettings >= 0)
                PowerManager.SetPlanSetting(
                    PowerSchemeGuid,
                    SettingSubgroup,
                    powerSchemeValues.Setting,
                    PowerMode.DC,
                    (uint)powerSchemeValues.DCSettings
                );
        }

        private void ApplyACValues(PowerSchemeValues powerSchemeValues)
        {
            if (PowerSchemeGuid == Guid.Empty)
                throw new ArgumentNullException(nameof(PowerSchemeGuid));

            if (powerSchemeValues.ACSettings >= 0)
                PowerManager.SetPlanSetting(
                    PowerSchemeGuid,
                    SettingSubgroup,
                    powerSchemeValues.Setting,
                    PowerMode.AC,
                    (uint)powerSchemeValues.ACSettings
                );
        }
    }
}