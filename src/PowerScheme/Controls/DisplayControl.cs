using System.Diagnostics;
using System.Windows.Forms;
using Languages;
using PowerScheme.Forms;
using PowerScheme.Model.Options;
using static PowerScheme.Addins.IdleMonitoring.Constants;

namespace PowerScheme.Controls;

public partial class DisplayControl : UserControl
{
    private readonly DisplayOptions _displayOptions = new();
    private readonly ToolTip _applyButtonToolTip = new();

    public DisplayControl()
    {
        InitializeComponent();

        SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.UserPaint |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw, true);

        InitializePowerSchemes();
        InitializeButtons();
        InitializeTurnOffDisplay();
        InitializeTurnOffLockedDisplay();
    }

    public string Title => Language.Current.IdleDisplayOptions;

    public void OnShown()
    {
        powerSchemeComboBox.SelectedValue = _displayOptions.ActivePowerSchemeId;
        ReviewApplyButton();
    }

    private void ResetDefaults()
    {
        var displayOptions = new DisplayOptions();

        turnOffDisplayNumericUpDown.Value = displayOptions.TurnOffDisplay;
        turnOffLockedDisplayNumericUpDown.Value = displayOptions.TurnOffLockDisplay;

        powerSchemeComboBox.SelectedValue = _displayOptions.DefaultPowerSchemeId;
    }

    private void InitializePowerSchemes() =>
        PowerSchemeControlBinding.BindSchemes(powerSchemeComboBox, powerSchemesLabel, _displayOptions, ReviewApplyButton);

    private void ReviewApplyButton()
    {
        Debug.WriteLine($"Power Scheme Id Changed: {_displayOptions.PowerSchemeId}");
        Debug.WriteLine($"AcEqualsDc: {_displayOptions.AcEqualsDc}");

        PowerSchemeControlBinding.ReviewApplyButton(applyButton, _applyButtonToolTip, _displayOptions.AcEqualsDc);
    }

    private void InitializeButtons() =>
        PowerSchemeControlBinding.BindButtons(okButton, applyButton, defaultButton, _displayOptions,
            () => WindowManager.HideForm<IdleDisplayForm>(), _displayOptions.ApplyDcAcValues, ReviewApplyButton, ResetDefaults);

    private void InitializeTurnOffDisplay()
    {
        turnOffDisplayLabel.Text = Language.Current.TurnOffDisplay;

        TimeSettingBinding.Bind(turnOffDisplayNumericUpDown, turnOffDisplayValueLabel, _displayOptions,
            nameof(_displayOptions.TurnOffDisplay), nameof(_displayOptions.TurnOffDisplayFormated),
            MIN_TURN_OFF_DISPLAY_IN_SECONDS, MAX_TURN_OFF_DISPLAY_IN_SECONDS);
    }

    private void InitializeTurnOffLockedDisplay()
    {
        turnOffLockedDisplayLabel.Text = Language.Current.TurnOffLockedDisplay;

        TimeSettingBinding.Bind(turnOffLockedDisplayNumericUpDown, turnOffLockedDisplayValueLabel, _displayOptions,
            nameof(_displayOptions.TurnOffLockDisplay), nameof(_displayOptions.TurnOffLockedDisplayFormated),
            MIN_TURN_OFF_LOCK_DISPLAY_IN_SECONDS, MAX_TURN_OFF_LOCK_DISPLAY_IN_SECONDS);
    }
}
