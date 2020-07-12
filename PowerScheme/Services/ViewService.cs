namespace PowerScheme.Services
{
    using Model;
    using RegistryManager.EventsArgs;
    using System;
    using System.Drawing;
    using System.Reflection;
    using System.Windows.Forms;
    using static Utility.TrayIcon;

    public partial class ViewService : IViewService, IDisposable
    {
        private const string SHOW_CONTEXT_MENU = "ShowContextMenu";
        private const string CHECK_ICON = "Check";
        private const string STOP_ICON = "Stop";
        private readonly IViewModel _viewModel;
        private readonly IPowerSchemeService _power;

        public ViewService(IViewModel viewModel, IPowerSchemeService power)
        {
            _viewModel = viewModel;
            _power = power;
        }

        public void Start()
        {
            _power.Watchers.PowerSchemes.Changed += ChangedPowerSchemes;
            _power.Watchers.ActivePowerScheme.Changed += ChangedActivePowerScheme;

            _viewModel.NotifyIcon.MouseClick += NotifyIcon_MouseClick;
            _viewModel.NotifyIcon.Visible = true;
            _viewModel.NotifyIcon.ContextMenuStrip = _viewModel.ContextLeftMenu;

            BuildMenu();
            UpdateIcon();

            _power.Watchers.Start();
        }

        public void Stop()
        {
            _power.Watchers.PowerSchemes.Changed -= ChangedPowerSchemes;
            _power.Watchers.ActivePowerScheme.Changed -= ChangedActivePowerScheme;

            _viewModel.NotifyIcon.Icon = GetIcon(STOP_ICON);
            _viewModel.NotifyIcon.Text = string.Empty;
            _viewModel.NotifyIcon.MouseClick -= NotifyIcon_MouseClick;

            _power.Watchers.Stop();
        }

        private void ChangedActivePowerScheme(object sender, RegistryChangedEventArgs e)
        {
            UpdateIcon();
        }

        private void UpdateIcon()
        {
            var activePowerScheme = _power.ActivePowerScheme;
            var image = activePowerScheme.Image;
            var icon = GetIcon(image);

            _viewModel.NotifyIcon.Icon = icon;
            _viewModel.NotifyIcon.Text = activePowerScheme.Name;
        }

        private void ChangedPowerSchemes(object sender, RegistryChangedEventArgs e)
        {
            BuildLeftMenu();
        }

        private void BuildMenu()
        {
            BuildLeftMenu();
            BuildRightMenu();
        }

        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            _viewModel.NotifyIcon.ContextMenuStrip =
            e.Button == MouseButtons.Right ?
                _viewModel.ContextRightMenu :
                _viewModel.ContextLeftMenu;

            if (e.Button == MouseButtons.Right)
            {
                CheckMenu(
                    _viewModel.ContextRightMenu.Items[STARTUP_ON_WINDOWS_MENU],
                    RegistryService.IsRunOnStartup);

                if (_power.IsHibernate)
                    CheckMenu(
                    _viewModel.ContextRightMenu.Items[SHOW_HIBERNATE_OPTION_MENU],
                    RegistryService.IsShowHibernateOption);

                CheckMenu(
                    _viewModel.ContextRightMenu.Items[SHOW_SLEEP_OPTION_MENU],
                    RegistryService.IsShowSleepOption);

                CheckLid();
            }

            var mi = typeof(NotifyIcon).GetMethod(SHOW_CONTEXT_MENU, BindingFlags.Instance | BindingFlags.NonPublic);
            mi?.Invoke(_viewModel.NotifyIcon, null);
            _viewModel.NotifyIcon.ContextMenuStrip = null;
        }

        private void CheckLid()
        {
            if (!_power.IsMobilePlatformRole) return;

            var any = _power.ActivePowerScheme.Guid;
            var valueLidOn = RegistryService.GetLidOption(any);
            var lidItems = _viewModel.ContextRightMenu.Items[LIDON_DROP_DOWN_MENU] as ToolStripMenuItem;
            var name = string.Empty;
            foreach (ToolStripMenuItem lidStripMenuItem in lidItems.DropDownItems)
            {
                var @checked = valueLidOn == (int)lidStripMenuItem.Tag;
                lidStripMenuItem.Image = GetImage(@checked ? RADIO_ON_ICON : RADIO_OFF_ICON);
                if (@checked) name = lidStripMenuItem.Name;
            }
            _viewModel.ContextRightMenu.Items[LIDON_DROP_DOWN_MENU].Image = GetImage(_imageLidOn[name]);
        }

        private void CheckMenu(ToolStripItem item, bool @checked)
        {
            item.Tag = @checked;
            item.Image = GetImageIfCheck(@checked);
        }

        private Bitmap GetImageIfCheck(bool @checked)
        {
            return @checked ? GetImage(CHECK_ICON) : null;
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue) { return; }
            if (disposing)
            {
                Stop();
                UnsubscribeFromContextLeftMenu();
                UnsubscribeFromContextRightMenu();
            }

            disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);

            //No destructor so isn't required (yet)            
            // GC.SuppressFinalize(this); 
        }

        #endregion
    }
}
