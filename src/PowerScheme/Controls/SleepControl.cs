using System.Diagnostics;
using System.Windows.Forms;
using Languages;
using PowerScheme.Forms;
using PowerScheme.Model.Options;
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

    private void ResetDefaults()
    {
        var sleepOptions = new SleepOptions();

        sleepNumericUpDown.Value = sleepOptions.SleepIdle;
        hibernateNumericUpDown.Value = sleepOptions.HibernateIdle;

        powerSchemeComboBox.SelectedValue = _sleepOptions.DefaultPowerSchemeId;
    }

    private void InitializePowerSchemes() =>
        PowerSchemeControlBinding.BindSchemes(powerSchemeComboBox, powerSchemesLabel, _sleepOptions, ReviewApplyButton);

    private void ReviewApplyButton()
    {
        Debug.WriteLine($"Power Scheme Id Changed: {_sleepOptions.PowerSchemeId}");
        Debug.WriteLine($"AcEqualsDc: {_sleepOptions.AcEqualsDc}");

        PowerSchemeControlBinding.ReviewApplyButton(applyButton, _applyButtonToolTip, _sleepOptions.AcEqualsDc);
    }

    private void InitializeButtons() =>
        PowerSchemeControlBinding.BindButtons(okButton, applyButton, defaultButton, _sleepOptions,
            () => WindowManager.HideForm<IdleSleepForm>(), _sleepOptions.ApplyDcAcValues, ReviewApplyButton, ResetDefaults);

    private void InitializeSleep()
    {
        sleepLabel.Text = Language.Current.SleepName;

        TimeSettingBinding.Bind(sleepNumericUpDown, sleepValueLabel, _sleepOptions,
            nameof(_sleepOptions.SleepIdle), nameof(_sleepOptions.SleepIdleFormated),
            MIN_SLEEP_IN_SECONDS, MAX_SLEEP_IN_SECONDS);
    }

    private void InitializeHibernate()
    {
        hibernateLabel.Text = Language.Current.HibernateName;

        TimeSettingBinding.Bind(hibernateNumericUpDown, hibernateValueLabel, _sleepOptions,
            nameof(_sleepOptions.HibernateIdle), nameof(_sleepOptions.HibernateIdleFormated),
            MIN_HIBERNATE_IN_SECONDS, MAX_HIBERNATE_IN_SECONDS);
    }
}
