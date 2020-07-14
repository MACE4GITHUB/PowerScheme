using Common;

namespace PowerScheme.Services
{
    using Model;
    using PowerSchemeServiceAPI;
    using RegistryManager;
    using RegistryManager.EventsArgs;
    using System.Drawing;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Forms;
    using static MenuLookup;
    using static Utility.TrayIcon;

    public partial class ViewService : IViewService
    {
        private const string SHOW_CONTEXT_MENU = "ShowContextMenu";
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

            _viewModel.NotifyIcon.Icon = GetIcon(ImageItem.Stop);
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
            var image = activePowerScheme.Picture;
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
                _viewModel.ContextRightMenu.Items[MenuItm.StartupOnWindows.ToString()],
                RegistryService.IsRunOnStartup);

            if (_power.ExistsHibernate)
                CheckMenu(
                    _viewModel.ContextRightMenu.Items[MenuItm.Hibernate.ToString()],
                    RegistryService.IsShowHibernateOption);

            CheckMenu(
                _viewModel.ContextRightMenu.Items[MenuItm.Sleep.ToString()],
                RegistryService.IsShowSleepOption);

            if (_viewModel.ContextRightMenu.Items[MenuItm.Settings.ToString()] is ToolStripMenuItem settingsToolStripMenuItem)
            {
                settingsToolStripMenuItem.DropDownItems[MenuItm.DeleteTypicalSchemes.ToString()].Visible = _power.UserPowerSchemes.Any();
                settingsToolStripMenuItem.DropDownItems[MenuItm.CreateTypicalSchemes.ToString()].Visible = !_power.ExistsAllTypicalScheme;
                UpdateItemsTypicalScheme();
            }

            CheckLid();
        }

        private void CheckLid()
        {
            if (!_power.ExistsMobilePlatformRole) return;

            var any = _power.ActivePowerScheme.Guid;
            var valueLidOn = RegistryService.GetLidOption(any);
            var lidItems = _viewModel.ContextRightMenu.Items[MenuItm.Lid.ToString()] as ToolStripMenuItem;
            ImageItem pictureName = ImageItem.Unknown;
            foreach (ToolStripMenuItem lidStripMenuItem in lidItems.DropDownItems)
            {
                var valueTag = (int)lidStripMenuItem.Tag;
                var @checked = valueLidOn == valueTag;
                // Uncomment if you want to change the Image style
                //lidStripMenuItem.Image = GetImage(@checked ? ImageItem.RadioOn : ImageItem.RadioOff);
                if (@checked) pictureName = MenuItems.Where(mi =>
                    {
                        var isLidSubMenu = mi.Key.ToString().StartsWith("Lid_");
                        if (!isLidSubMenu) return false;
                        if (!(mi.Value.Tag is int value)) return false;
                        return value == valueTag;
                    }).Select(mi => mi.Value.Picture).FirstOrDefault();
            }
            _viewModel.ContextRightMenu.Items[MenuItm.Lid.ToString()].Image = GetImage(pictureName);
        }

        private void CheckMenu(ToolStripItem item, bool @checked)
        {
            item.Tag = @checked;
            item.Image = GetImageIfCheck(@checked);
        }

        private Bitmap GetImageIfCheck(bool @checked)
        {
            return @checked ? GetImage(ImageItem.Check) : null;
        }
    }
}
