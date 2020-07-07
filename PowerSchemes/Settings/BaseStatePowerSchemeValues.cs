using PowerManagerAPI;
using System;

namespace PowerSchemes.Settings
{
    public abstract class BaseStatePowerSchemeValues: IApplicable
    {
        public BaseStatePowerSchemeValues(Guid powerSchemeGuid)
        {
            PowerSchemeGuid = powerSchemeGuid;
        }

        //public virtual PowerSchemeValues defaultDCACValues { get; }
        public virtual PowerSchemeValues State { get; }

        public virtual SettingSubgroup SettingSubgroup { get; }

        public Guid PowerSchemeGuid { get; }


        public void ApplyValues()
        {
            ApplyDCStateValues();
            ApplyACStateValues();
        }

        private void ApplyDCStateValues()
        {
            ApplyDCValues(State);
        }

        private void ApplyACStateValues()
        {
            ApplyACValues(State);
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