using System;
using PowerManagerAPI;

namespace PowerSchemeServiceAPI.Settings;

public abstract class BaseMinMaxPowerSchemeValues : IApplicable
{
    protected BaseMinMaxPowerSchemeValues(Guid powerSchemeGuid) => 
        PowerSchemeGuid = powerSchemeGuid;

    protected virtual PowerSchemeValues MinState { get; set; }

    protected virtual PowerSchemeValues MaxState { get; set; }

    protected virtual SettingSubgroup SettingSubgroup { get; }

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