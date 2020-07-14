﻿namespace PowerScheme.Services
{
    using Model;
    using PowerSchemeServiceAPI;
    using RegistryManager;
    using RegistryManager.EventsArgs;
    using System.Drawing;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Forms;
    using static Utility.TrayIcon;

    public partial class ViewService : IViewService
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
            UnsubscribeFromContextLeftMenu();
            UnsubscribeFromContextRightMenu();
        }

        private void ChangedActivePowerScheme(object sender, RegistryChangedEventArgs e)
        {
            UpdateIcon();
        }

        private void UpdateIcon()
        {
            var activePowerScheme = _power.ActivePowerScheme;
            var image = activePowerScheme.PictureName;
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
                UpdateContextRightMenu();
            }

            var mi = typeof(NotifyIcon).GetMethod(SHOW_CONTEXT_MENU, BindingFlags.Instance | BindingFlags.NonPublic);
            mi?.Invoke(_viewModel.NotifyIcon, null);
            _viewModel.NotifyIcon.ContextMenuStrip = null;
        }

        private void UpdateContextRightMenu()
        {
            CheckMenu(
                _viewModel.ContextRightMenu.Items[STARTUP_ON_WINDOWS_MENU],
                RegistryService.IsRunOnStartup);

            if (_power.ExistsHibernate)
                CheckMenu(
                    _viewModel.ContextRightMenu.Items[SHOW_HIBERNATE_OPTION_MENU],
                    RegistryService.IsShowHibernateOption);

            CheckMenu(
                _viewModel.ContextRightMenu.Items[SHOW_SLEEP_OPTION_MENU],
                RegistryService.IsShowSleepOption);

            if (_viewModel.ContextRightMenu.Items[SETTINGS_DROP_DOWN_MENU] is ToolStripMenuItem settingsToolStripMenuItem)
            {
                settingsToolStripMenuItem.DropDownItems[DELETE_TYPICAL_SCHEMES_MENU].Visible = _power.UserPowerSchemes.Any();
                settingsToolStripMenuItem.DropDownItems[ADD_TYPICAL_SCHEMES_MENU].Visible = !_power.ExistsAllTypicalScheme;
                UpdateItemsTypicalScheme();
            }

            CheckLid();
        }

        private void CheckLid()
        {
            if (!_power.ExistsMobilePlatformRole) return;

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
            _viewModel.ContextRightMenu.Items[LIDON_DROP_DOWN_MENU].Image = GetImage(IMAGE_LID_ON[name]);
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
    }
}
