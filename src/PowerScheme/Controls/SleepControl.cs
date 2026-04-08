using System;
using System.Diagnostics;
using System.Windows.Forms;
using Languages;
using PowerScheme.Forms;
using PowerScheme.Model.Options;
using PowerScheme.Themes;
using static PowerScheme.Addins.IdleMonitoring.Constants;

namespace PowerScheme.Controls;

public partial class SleepControl : UserControl
{
    private readonly SleepOptions _sleepOptions = new();
    private readonly ToolTip _applyButtonToolTip = new();

    public SleepControl()
    {
        InitializeComponent();

        SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.UserPaint |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw, true);

        InitializePowerSchemes();
        InitializeButtons();
        InitializeSleep();
        InitializeHibernate();
    }

    public string Title => Language.Current.IdleSleepOptions;

    public void OnShown()
    {
        powerSchemeComboBox.SelectedValue = _sleepOptions.ActivePowerSchemeId;
        ReviewApplyButton();
    }

    private void OkButtonOnClick(object sender, EventArgs e)
    {
        WindowManager.HideForm<IdleSleepForm>();
        _sleepOptions.ApplyDcAcValues();
    }

    private void DefaultButtonOnClick(object sender, EventArgs e)
    {
        var sleepOptions = new SleepOptions();

        sleepNumericUpDown.Value = sleepOptions.SleepIdle;
        hibernateNumericUpDown.Value = sleepOptions.HibernateIdle;

        powerSchemeComboBox.SelectedValue = _sleepOptions.DefaultPowerSchemeId;
    }

    private void ApplyButtonOnClick(object sender, EventArgs e)
    {
        Debug.WriteLine($"Selected Power Scheme Id: {_sleepOptions.PowerSchemeId}");

        _sleepOptions.ApplyDcAcValues();
        ReviewApplyButton();
    }

    private void InitializePowerSchemes()
    {
        _sleepOptions.PowerSchemeIdChanged += (_, _) => ReviewApplyButton();

        powerSchemesLabel.Text = Language.Current.PowerSchemes;

        var binding = new BindingSource
        {
            DataSource = _sleepOptions.PowerSchemes
        };

        binding.ResetBindings(false);
        powerSchemeComboBox.DataSource = binding;
        powerSchemeComboBox.DisplayMember = "Value";
        powerSchemeComboBox.ValueMember = "Key";
        powerSchemeComboBox.DataBindings.Add(nameof(powerSchemeComboBox.SelectedValue), _sleepOptions, nameof(_sleepOptions.PowerSchemeId), true, DataSourceUpdateMode.OnPropertyChanged);
    }

    private void ReviewApplyButton()
    {
        Debug.WriteLine($"Power Scheme Id Changed: {_sleepOptions.PowerSchemeId}");
        Debug.WriteLine($"AcEqualsDc: {_sleepOptions.AcEqualsDc}");

        if (_sleepOptions.AcEqualsDc)
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

    private void InitializeSleep()
    {
        sleepLabel.Text = Language.Current.SleepName;

        sleepNumericUpDown.DataBindings.Add(nameof(sleepNumericUpDown.Value), _sleepOptions, nameof(_sleepOptions.SleepIdle), false, DataSourceUpdateMode.OnPropertyChanged);
        sleepValueLabel.DataBindings.Add(nameof(sleepValueLabel.Text), _sleepOptions, nameof(_sleepOptions.SleepIdleFormated));

        sleepNumericUpDown.KeyUp += SleepNumericUpDownOnKeyUp;

        sleepNumericUpDown.Minimum = MIN_SLEEP_IN_SECONDS;
        sleepNumericUpDown.Maximum = MAX_SLEEP_IN_SECONDS;

    }

    private void InitializeHibernate()
    {
        hibernateLabel.Text = Language.Current.HibernateName;

        hibernateNumericUpDown.DataBindings.Add(nameof(hibernateNumericUpDown.Value), _sleepOptions, nameof(_sleepOptions.HibernateIdle), false, DataSourceUpdateMode.OnPropertyChanged);
        hibernateValueLabel.DataBindings.Add(nameof(hibernateValueLabel.Text), _sleepOptions, nameof(_sleepOptions.HibernateIdleFormated));

        hibernateNumericUpDown.KeyUp += HibernateNumericUpDownOnKeyUp;

        hibernateNumericUpDown.Minimum = MIN_HIBERNATE_IN_SECONDS;
        hibernateNumericUpDown.Maximum = MAX_HIBERNATE_IN_SECONDS;
    }

    private void SleepNumericUpDownOnKeyUp(object? sender, KeyEventArgs e) =>
        sleepValueLabel.Text = OptionsHelper.FormatHoursMinutesSeconds(TimeSpan.FromSeconds((int)sleepNumericUpDown.Value));

    private void HibernateNumericUpDownOnKeyUp(object? sender, KeyEventArgs e) =>
        hibernateValueLabel.Text = OptionsHelper.FormatHoursMinutesSeconds(TimeSpan.FromSeconds((int)hibernateNumericUpDown.Value));
}
