using System;
using PowerManagerAPI;

namespace PowerSchemeServiceAPI.Settings;

public abstract class BaseStatePowerSchemeValues: IApplicable
{
    protected BaseStatePowerSchemeValues(Guid powerSchemeGuid)
    {
        PowerSchemeGuid = powerSchemeGuid;
    }

    protected virtual PowerSchemeValues State { get; }

    protected virtual SettingSubgroup SettingSubgroup { get; }

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
        if (powerSchemeValues.DCSettings == -1) return;

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
        if (powerSchemeValues.ACSettings == -1) return;

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