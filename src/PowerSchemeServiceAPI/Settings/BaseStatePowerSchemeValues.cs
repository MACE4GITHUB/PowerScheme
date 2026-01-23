using System;
using PowerManagerAPI;

namespace PowerSchemeServiceAPI.Settings;

public abstract class BaseStatePowerSchemeValues(
    Guid powerSchemeGuid) :
    IApplicable
{
    protected abstract PowerSchemeValues State { get; }

    protected abstract SettingSubgroup SettingSubgroup { get; }

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
        if (powerSchemeValues.DcSettings == -1)
        {
            return;
        }

        if (PowerSchemeGuid == Guid.Empty)
        {
            throw new AccessViolationException(nameof(PowerSchemeGuid));
        }

        if (powerSchemeValues.DcSettings >= 0)
        {
            PowerManager.SetPlanSetting(
                PowerSchemeGuid,
                SettingSubgroup,
                powerSchemeValues.Setting,
                PowerMode.DC,
                (uint)powerSchemeValues.DcSettings
            );
        }
    }

    private void ApplyAcValues(PowerSchemeValues powerSchemeValues)
    {
        if (powerSchemeValues.AcSettings == -1)
        {
            return;
        }

        if (PowerSchemeGuid == Guid.Empty)
        {
            throw new AccessViolationException(nameof(PowerSchemeGuid));
        }

        if (powerSchemeValues.AcSettings >= 0)
        {
            PowerManager.SetPlanSetting(
                PowerSchemeGuid,
                SettingSubgroup,
                powerSchemeValues.Setting,
                PowerMode.AC,
                (uint)powerSchemeValues.AcSettings
            );
        }
    }
}
