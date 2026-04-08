using System;
using System.Windows.Forms;
using Languages;
using PowerScheme.Configuration;
using PowerScheme.Forms;
using PowerScheme.Model;
using PowerScheme.Model.Options;
using PowerScheme.Services;
using RegistryManager;

namespace PowerScheme.Controls;

public partial class ThresholdControl : UserControl
{
    private readonly IdleOptions _idleOptions = new();

    private readonly IIdleMonitorService _idleMonitorService = DiRoot.GetService<IIdleMonitorService>();

    public ThresholdControl()
    {
        InitializeComponent();

        SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.UserPaint |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw, true);

        InitializeButtons();
        InitializeIdleThreshold();
        InitializePollingActiveTime();
        InitializePollingIdleTime();
    }

    public string Title => Language.Current.IdleOptions;

    public void OnShown()
    {
        var idleThresholdInMilliseconds = RegistryService.GetIdleThresholdInMilliseconds(AppInfo.CompanyName, AppInfo.ProductName);
        var pollingActiveTimeInMilliseconds = RegistryService.GetPollingActiveTimeInMilliseconds(AppInfo.CompanyName, AppInfo.ProductName);
        var pollingIdleTimeInMilliseconds = RegistryService.GetPollingIdleTimeInMilliseconds(AppInfo.CompanyName, AppInfo.ProductName);

        var idleOptions = new IdleOptions(idleThresholdInMilliseconds, pollingActiveTimeInMilliseconds, pollingIdleTimeInMilliseconds);

        idleThresholdTrackBar.Value = idleOptions.IdleThreshold;
        pollingActiveTimeTrackBar.Value = idleOptions.PollingActiveTime;
        pollingIdleTimeTrackBar.Value = idleOptions.PollingIdleTime;
    }

    private void OkButtonOnClick(object sender, EventArgs e)
    {
        WindowManager.HideForm<IdleForm>();

        RegistryService.SetIdleThresholdInMilliseconds(AppInfo.CompanyName, AppInfo.ProductName, _idleOptions.IdleThresholdMs);
        RegistryService.SetPollingActiveTimeInMilliseconds(AppInfo.CompanyName, AppInfo.ProductName, _idleOptions.PollingActiveTimeMs);
        RegistryService.SetPollingIdleTimeInMilliseconds(AppInfo.CompanyName, AppInfo.ProductName, _idleOptions.PollingIdleTimeMs);

        _idleMonitorService.Restart();
    }

    private void DefaultButtonOnClick(object sender, EventArgs e)
    {
        var idleOptions = new IdleOptions();

        idleThresholdTrackBar.Value = idleOptions.IdleThreshold;
        pollingActiveTimeTrackBar.Value = idleOptions.PollingActiveTime;
        pollingIdleTimeTrackBar.Value = idleOptions.PollingIdleTime;
    }

    private void InitializeButtons()
    {
        okButton.Text = Language.Current.Ok;
        defaultButton.Text = Language.Current.Default;

        okButton.Click += OkButtonOnClick;
        defaultButton.Click += DefaultButtonOnClick;
    }

    private void InitializeIdleThreshold()
    {
        idleThresholdLabel.Text = Language.Current.IdleThreshold;
        var tip = new ToolTip();
        tip.SetToolTip(idleThresholdLabel, Language.Current.TheUserIdleTimeAfterWhichThePowerPlanWillChange);

        idleThresholdTrackBar.DataBindings.Add(nameof(idleThresholdTrackBar.Value), _idleOptions, nameof(_idleOptions.IdleThreshold), false, DataSourceUpdateMode.OnPropertyChanged);
        idleThresholdValueLabel.DataBindings.Add(nameof(idleThresholdValueLabel.Text), _idleOptions, nameof(_idleOptions.IdleThresholdMsFormated));
    }

    private void InitializePollingActiveTime()
    {
        pollingActiveTimeLabel.Text = Language.Current.PollingActiveTime;
        var tip = new ToolTip();
        tip.SetToolTip(pollingActiveTimeLabel, Language.Current.MonitoringFrequencyWhenTheUserIsActive);

        pollingActiveTimeTrackBar.DataBindings.Add(nameof(pollingActiveTimeTrackBar.Value), _idleOptions, nameof(_idleOptions.PollingActiveTime), false, DataSourceUpdateMode.OnPropertyChanged);
        pollingActiveTimeValueLabel.DataBindings.Add(nameof(pollingActiveTimeValueLabel.Text), _idleOptions, nameof(_idleOptions.PollingActiveTimeMsFormated));
    }

    private void InitializePollingIdleTime()
    {
        pollingIdleTimeLabel.Text = Language.Current.PollingIdleTime;
        var tip = new ToolTip();
        tip.SetToolTip(pollingIdleTimeLabel, Language.Current.MonitoringFrequencyDuringUserInactivity);

        pollingIdleTimeTrackBar.DataBindings.Add(nameof(pollingIdleTimeTrackBar.Value), _idleOptions, nameof(_idleOptions.PollingIdleTime), false, DataSourceUpdateMode.OnPropertyChanged);
        pollingIdleTimeValueLabel.DataBindings.Add(nameof(pollingIdleTimeValueLabel.Text), _idleOptions, nameof(_idleOptions.PollingIdleTimeMsFormated));
    }
}