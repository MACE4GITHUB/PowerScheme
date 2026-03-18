using System;
using System.Windows.Forms;
using Languages;
using PowerScheme.Configuration;
using PowerScheme.Model;
using PowerScheme.Model.Options;
using PowerScheme.Services;
using RegistryManager;

namespace PowerScheme.Forms
{
    public partial class IdleForm : Form
    {
        private readonly IdleOptions _idleOptions = new();

        private readonly IIdleMonitorService _idleMonitorService = DiRoot.GetService<IIdleMonitorService>();

        public IdleForm()
        {
            InitializeComponent();

            Deactivate += OnDeactivate;
            Activated += OnActivated;
            okButton.Click += (_, _) => WindowManager.HideForm<IdleForm>();
            defaultButton.Click += DefaultButtonOnClick;

            captionLabel.Text = Language.Current.IdleOptions;
            okButton.Text = Language.Current.Ok;
            defaultButton.Text = Language.Current.Default;

            AddIdleThreshold();
            AddPollingActiveTime();
            AddPollingIdleTime();
        }

        private void DefaultButtonOnClick(object sender, EventArgs e)
        {
            var idleOptions = new IdleOptions();

            idleThresholdTrackBar.Value = idleOptions.IdleThreshold;
            pollingActiveTimeTrackBar.Value = idleOptions.PollingActiveTime;
            pollingIdleTimeTrackBar.Value = idleOptions.PollingIdleTime;
        }

        private void AddIdleThreshold()
        {
            idleThresholdLabel.Text = Language.Current.IdleThreshold;
            var tip = new ToolTip();
            tip.SetToolTip(idleThresholdLabel, Language.Current.TheUserIdleTimeAfterWhichThePowerPlanWillChange);

            idleThresholdTrackBar.DataBindings.Add(nameof(idleThresholdTrackBar.Value), _idleOptions, nameof(_idleOptions.IdleThreshold), false, DataSourceUpdateMode.OnPropertyChanged);
            idleThresholdValueLabel.DataBindings.Add(nameof(idleThresholdValueLabel.Text), _idleOptions, nameof(_idleOptions.IdleThresholdMsFormated));
        }

        private void AddPollingActiveTime()
        {
            pollingActiveTimeLabel.Text = Language.Current.PollingActiveTime;
            var tip = new ToolTip();
            tip.SetToolTip(pollingActiveTimeLabel, Language.Current.MonitoringFrequencyWhenTheUserIsActive);

            pollingActiveTimeTrackBar.DataBindings.Add(nameof(pollingActiveTimeTrackBar.Value), _idleOptions, nameof(_idleOptions.PollingActiveTime), false, DataSourceUpdateMode.OnPropertyChanged);
            pollingActiveTimeValueLabel.DataBindings.Add(nameof(pollingActiveTimeValueLabel.Text), _idleOptions, nameof(_idleOptions.PollingActiveTimeMsFormated));
        }

        private void AddPollingIdleTime()
        {
            pollingIdleTimeLabel.Text = Language.Current.PollingIdleTime;
            var tip = new ToolTip();
            tip.SetToolTip(pollingIdleTimeLabel, Language.Current.MonitoringFrequencyDuringUserInactivity);

            pollingIdleTimeTrackBar.DataBindings.Add(nameof(pollingIdleTimeTrackBar.Value), _idleOptions, nameof(_idleOptions.PollingIdleTime), false, DataSourceUpdateMode.OnPropertyChanged);
            pollingIdleTimeValueLabel.DataBindings.Add(nameof(pollingIdleTimeValueLabel.Text), _idleOptions, nameof(_idleOptions.PollingIdleTimeMsFormated));
        }

        private void OnDeactivate(object sender, EventArgs e)
        {
            WindowManager.HideForm<IdleForm>();

            RegistryService.SetIdleThresholdInMilliseconds(AppInfo.CompanyName, AppInfo.ProductName, _idleOptions.IdleThresholdMs);
            RegistryService.SetPollingActiveTimeInMilliseconds(AppInfo.CompanyName, AppInfo.ProductName, _idleOptions.PollingActiveTimeMs);
            RegistryService.SetPollingIdleTimeInMilliseconds(AppInfo.CompanyName, AppInfo.ProductName, _idleOptions.PollingIdleTimeMs);

            _idleMonitorService.Restart();
        }

        private void OnActivated(object sender, EventArgs e)
        {
            var idleThresholdInMilliseconds = RegistryService.GetIdleThresholdInMilliseconds(AppInfo.CompanyName, AppInfo.ProductName);
            var pollingActiveTimeInMilliseconds = RegistryService.GetPollingActiveTimeInMilliseconds(AppInfo.CompanyName, AppInfo.ProductName);
            var pollingIdleTimeInMilliseconds = RegistryService.GetPollingIdleTimeInMilliseconds(AppInfo.CompanyName, AppInfo.ProductName);

            var idleOptions = new IdleOptions(idleThresholdInMilliseconds, pollingActiveTimeInMilliseconds, pollingIdleTimeInMilliseconds);

            idleThresholdTrackBar.Value = idleOptions.IdleThreshold;
            pollingActiveTimeTrackBar.Value = idleOptions.PollingActiveTime;
            pollingIdleTimeTrackBar.Value = idleOptions.PollingIdleTime;
        }
    }
}
