using System;
using PowerManagerAPI;

namespace PowerSchemeServiceAPI.Settings;

public abstract class BaseMinMaxPowerSchemeValues(
    Guid powerSchemeGuid) :
    IApplicable
{
    protected abstract PowerSchemeValues MinState { get; }

    protected abstract PowerSchemeValues MaxState { get; }

    protected abstract SettingSubgroup SettingSubgroup { get; }

    public Guid PowerSchemeGuid { get; } = powerSchemeGuid;

    public void ApplyValues()
    {
        ApplyDcMinMaxValues();
        ApplyAcMinMaxValues();
    }

    private void ApplyDcMinMaxValues()
    {
        ApplyDcValues(MinState);
        ApplyDcValues(MaxState);
    }

    private void ApplyAcMinMaxValues()
    {
        ApplyAcValues(MinState);
        ApplyAcValues(MaxState);
    }

    private void ApplyDcValues(PowerSchemeValues powerSchemeValues)
    {
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
