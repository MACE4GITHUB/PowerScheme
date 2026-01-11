using System;
using PowerManagerAPI;

namespace PowerSchemeServiceAPI.Settings;

public abstract class BaseStatePowerSchemeValues(
    Guid powerSchemeGuid) :
    IApplicable
{
    protected virtual PowerSchemeValues State { get; }

    protected virtual SettingSubgroup SettingSubgroup { get; }

    public Guid PowerSchemeGuid { get; } = powerSchemeGuid;

    public void ApplyValues()
    {
        ApplyDcStateValues();
        ApplyAcStateValues();
    }

    private void ApplyDcStateValues()
    {
        ApplyDcValues(State);
    }

    private void ApplyAcStateValues()
    {
        ApplyAcValues(State);
    }

    private void ApplyDcValues(PowerSchemeValues powerSchemeValues)
    {
        if (powerSchemeValues.DCSettings == -1)
        {
            return;
        }

        if (PowerSchemeGuid == Guid.Empty)
        {
            throw new AccessViolationException(nameof(PowerSchemeGuid));
        }

        if (powerSchemeValues.DCSettings >= 0)
        {
            PowerManager.SetPlanSetting(
                PowerSchemeGuid,
                SettingSubgroup,
                powerSchemeValues.Setting,
                PowerMode.DC,
                (uint)powerSchemeValues.DCSettings
            );
        }
    }

    private void ApplyAcValues(PowerSchemeValues powerSchemeValues)
    {
        if (powerSchemeValues.ACSettings == -1)
        {
            return;
        }

        if (PowerSchemeGuid == Guid.Empty)
        {
            throw new AccessViolationException(nameof(PowerSchemeGuid));
        }

        if (powerSchemeValues.ACSettings >= 0)
        {
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
