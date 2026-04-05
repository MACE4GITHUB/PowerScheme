using System;
using System.Diagnostics;
using System.Windows.Forms;
using Languages;
using PowerScheme.Forms;
using PowerScheme.Model.Options;
using PowerScheme.Themes;

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

    private void OkButtonOnClick(object sender, EventArgs e)
    {
        WindowManager.HideForm<IdleDisplayForm>();
        _displayOptions.ApplyDcAcValues();
    }

    private void DefaultButtonOnClick(object sender, EventArgs e)
    {
        var displayOptions = new DisplayOptions();

        turnOffDisplayNumericUpDown.Value = displayOptions.TurnOffDisplay;
        turnOffLockedDisplayNumericUpDown.Value = displayOptions.TurnOffLockDisplay;

        powerSchemeComboBox.SelectedValue = _displayOptions.DefaultPowerSchemeId;
    }

    private void ApplyButtonOnClick(object sender, EventArgs e)
    {
        Debug.WriteLine($"Selected Power Scheme Id: {_displayOptions.PowerSchemeId}");

        _displayOptions.ApplyDcAcValues();
        ReviewApplyButton();
    }

    private void InitializePowerSchemes()
    {
        _displayOptions.PowerSchemeIdChanged += (_, _) => ReviewApplyButton();

        powerSchemesLabel.Text = Language.Current.PowerSchemes;

        var binding = new BindingSource
        {
            DataSource = _displayOptions.PowerSchemes
        };

        binding.ResetBindings(false);
        powerSchemeComboBox.DataSource = binding;
        powerSchemeComboBox.DisplayMember = "Value";
        powerSchemeComboBox.ValueMember = "Key";
        powerSchemeComboBox.DataBindings.Add(nameof(powerSchemeComboBox.SelectedValue), _displayOptions, nameof(_displayOptions.PowerSchemeId), true, DataSourceUpdateMode.OnPropertyChanged);
    }

    private void ReviewApplyButton()
    {
        Debug.WriteLine($"Power Scheme Id Changed: {_displayOptions.PowerSchemeId}");
        Debug.WriteLine($"AcEqualsDc: {_displayOptions.AcEqualsDc}");

        if (_displayOptions.AcEqualsDc)
        {
            ThemeService.SetButtonTheme(applyButton);
            _applyButtonToolTip.SetToolTip(applyButton, null);
        }
        else
        {
            ThemeService.SetAttentionButtonTheme(applyButton);
            _applyButtonToolTip.SetToolTip(applyButton, Language.Current.SyncAcDc);
        }
    }

    private void InitializeButtons()
    {
        okButton.Text = Language.Current.Ok;
        applyButton.Text = Language.Current.Apply;
        defaultButton.Text = Language.Current.Default;

        okButton.Click += OkButtonOnClick;
        applyButton.Click += ApplyButtonOnClick;
        defaultButton.Click += DefaultButtonOnClick;

        defaultButton.Visible = false;
    }

    private void InitializeTurnOffDisplay()
    {
        turnOffDisplayLabel.Text = Language.Current.TurnOffDisplay;

        turnOffDisplayNumericUpDown.DataBindings.Add(nameof(turnOffDisplayNumericUpDown.Value), _displayOptions, nameof(_displayOptions.TurnOffDisplay), false, DataSourceUpdateMode.OnPropertyChanged);
        turnOffDisplayValueLabel.DataBindings.Add(nameof(turnOffDisplayValueLabel.Text), _displayOptions, nameof(_displayOptions.TurnOffDisplayFormated));

        turnOffDisplayNumericUpDown.KeyUp += TurnOffDisplayNumericUpDownOnKeyUp;
    }

    private void InitializeTurnOffLockedDisplay()
    {
        turnOffLockedDisplayLabel.Text = Language.Current.TurnOffLockedDisplay;

        turnOffLockedDisplayNumericUpDown.DataBindings.Add(nameof(turnOffLockedDisplayNumericUpDown.Value), _displayOptions, nameof(_displayOptions.TurnOffLockDisplay), false, DataSourceUpdateMode.OnPropertyChanged);
        turnOffLockedDisplayValueLabel.DataBindings.Add(nameof(turnOffLockedDisplayValueLabel.Text), _displayOptions, nameof(_displayOptions.TurnOffLockedDisplayFormated));

        turnOffLockedDisplayNumericUpDown.KeyUp += TurnOffLockedDisplayNumericUpDownOnKeyUp;
    }

    private void TurnOffDisplayNumericUpDownOnKeyUp(object? sender, KeyEventArgs e) =>
        turnOffDisplayValueLabel.Text = OptionsHelper.FormatHoursMinutesSeconds(TimeSpan.FromSeconds((int)turnOffDisplayNumericUpDown.Value));

    private void TurnOffLockedDisplayNumericUpDownOnKeyUp(object? sender, KeyEventArgs e) =>
        turnOffLockedDisplayValueLabel.Text = OptionsHelper.FormatHoursMinutesSeconds(TimeSpan.FromSeconds((int)turnOffLockedDisplayNumericUpDown.Value));
}
