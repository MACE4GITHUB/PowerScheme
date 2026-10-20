using System;
using PowerScheme.Configuration;
using PowerSchemeServiceAPI;
using PowerSchemeServiceAPI.Settings;
using static PowerScheme.Addins.IdleMonitoring.Constants;

namespace PowerScheme.Model.Options;

internal class DisplayOptions : PowerSchemeOptions
{
    public DisplayOptions(
        int turnOffDisplayInSc = DEFAULT_TURN_OFF_DISPLAY_IN_SECONDS,
        int turnOffLockDisplayInSc = DEFAULT_TURN_OFF_LOCK_DISPLAY_IN_SECONDS)
        : this(DiRoot.GetService<IPowerSchemeService>(), turnOffDisplayInSc, turnOffLockDisplayInSc)
    {
    }

    internal DisplayOptions(
        IPowerSchemeService power,
        int turnOffDisplayInSc = DEFAULT_TURN_OFF_DISPLAY_IN_SECONDS,
        int turnOffLockDisplayInSc = DEFAULT_TURN_OFF_LOCK_DISPLAY_IN_SECONDS)
        : base(power)
    {
        TurnOffDisplay = turnOffDisplayInSc;
        TurnOffLockDisplay = turnOffLockDisplayInSc;

        PowerSchemeId = ActivePowerSchemeId;
    }

    public int TurnOffDisplay { get; set; }

    public string TurnOffDisplayFormated => OptionsHelper.FormatHoursMinutesSeconds(TimeSpan.FromSeconds(TurnOffDisplay));

    public int TurnOffLockDisplay { get; set; }

    public string TurnOffLockedDisplayFormated => OptionsHelper.FormatHoursMinutesSeconds(TimeSpan.FromSeconds(TurnOffLockDisplay));

    public bool AcEqualsDc =>
        TurnOffDisplayDcAcValues.AcSettings == TurnOffDisplayDcAcValues.DcSettings &&
        TurnOffLockDisplayDcAcValues.AcSettings == TurnOffLockDisplayDcAcValues.DcSettings;

    public PowerSchemeDcAcValues TurnOffDisplayDcAcValues =>
        Power.GetIdleDisplay(EffectivePowerSchemeId);

    public void ApplyTurnOffDisplayDcAcValues() =>
        ApplyValue(TurnOffDisplay, Power.SetIdleDisplay, Power.SetAllPowerSchemesIdleDisplay);

    public PowerSchemeDcAcValues TurnOffLockDisplayDcAcValues =>
        Power.GetIdleLockDisplay(EffectivePowerSchemeId);

    public void ApplyTurnOffLockDisplayDcAcValues() =>
        ApplyValue(TurnOffLockDisplay, Power.SetIdleLockDisplay, Power.SetAllPowerSchemesIdleLockDisplay);

    public void ApplyDcAcValues()
    {
        ApplyTurnOffDisplayDcAcValues();
        ApplyTurnOffLockDisplayDcAcValues();
    }

    protected override void LoadSelectedValues()
    {
        TurnOffDisplay = TurnOffDisplayDcAcValues.AcSettings;
        TurnOffLockDisplay = TurnOffLockDisplayDcAcValues.AcSettings;
    }
}
