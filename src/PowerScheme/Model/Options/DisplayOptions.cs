using System;
using System.Collections.Generic;
using System.ComponentModel;
using Languages;
using PowerScheme.Configuration;
using PowerSchemeServiceAPI;
using PowerSchemeServiceAPI.Settings;
using static PowerScheme.Addins.IdleMonitoring.Constants;

namespace PowerScheme.Model.Options;

internal class DisplayOptions : INotifyPropertyChanged
{
    private readonly IPowerSchemeService _power = DiRoot.GetService<IPowerSchemeService>();

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler? PowerSchemeIdChanged;

    public DisplayOptions(
        int turnOffDisplayInSc = DEFAULT_TURN_OFF_DISPLAY_IN_SECONDS,
        int turnOffLockDisplayInSc = DEFAULT_TURN_OFF_LOCK_DISPLAY_IN_SECONDS)
    {
        TurnOffDisplay = turnOffDisplayInSc;
        TurnOffLockDisplay = turnOffLockDisplayInSc;

        PowerSchemeId = ActivePowerSchemeId;
    }

    public Dictionary<Guid, string> PowerSchemes
    {
        get
        {
            var powerSchemes = new Dictionary<Guid, string>
            {
                { Guid.Empty, Language.Current.AllPowerSchemes }
            };

            foreach (var powerScheme in _power.PowerSchemes)
            {
                powerSchemes.Add(powerScheme.Guid, powerScheme.Name);
            }

            return powerSchemes;
        }
    }

    public Guid PowerSchemeId
    {
        get => field;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;

            TurnOffDisplay = TurnOffDisplayDcAcValues.AcSettings;
            TurnOffLockDisplay = TurnOffLockDisplayDcAcValues.AcSettings;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PowerSchemeId)));
            PowerSchemeIdChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public Guid DefaultPowerSchemeId => Guid.Empty;

    public Guid ActivePowerSchemeId => _power.ActivePowerScheme.Guid;

    public int TurnOffDisplay { get; set; }

    public string TurnOffDisplayFormated => OptionsHelper.FormatHoursMinutesSeconds(TimeSpan.FromSeconds(TurnOffDisplay));

    public int TurnOffLockDisplay { get; set; }

    public string TurnOffLockedDisplayFormated => OptionsHelper.FormatHoursMinutesSeconds(TimeSpan.FromSeconds(TurnOffLockDisplay));

    public bool AcEqualsDc =>
        TurnOffDisplayDcAcValues.AcSettings == TurnOffDisplayDcAcValues.DcSettings &&
        TurnOffLockDisplayDcAcValues.AcSettings == TurnOffLockDisplayDcAcValues.DcSettings;

    public PowerSchemeDcAcValues TurnOffDisplayDcAcValues =>
        PowerSchemeId == Guid.Empty
            ? _power.GetIdleDisplay(ActivePowerSchemeId)
            : _power.GetIdleDisplay(PowerSchemeId);

    public void ApplyTurnOffDisplayDcAcValues()
    {
        if (PowerSchemeId == Guid.Empty)
        {
            _power.SetAllPowerSchemesIdleDisplay(TurnOffDisplay);

            return;
        }

        _power.SetIdleDisplay(PowerSchemeId, TurnOffDisplay);
    }

    public PowerSchemeDcAcValues TurnOffLockDisplayDcAcValues =>
        PowerSchemeId == Guid.Empty
            ? _power.GetIdleLockDisplay(ActivePowerSchemeId)
            : _power.GetIdleLockDisplay(PowerSchemeId);

    public void ApplyTurnOffLockDisplayDcAcValues()
    {
        if (PowerSchemeId == Guid.Empty)
        {
            _power.SetAllPowerSchemesIdleLockDisplay(TurnOffLockDisplay);

            return;
        }

        _power.SetIdleLockDisplay(PowerSchemeId, TurnOffLockDisplay);
    }

    public void ApplyDcAcValues()
    {
        ApplyTurnOffDisplayDcAcValues();
        ApplyTurnOffLockDisplayDcAcValues();
    }
}
