using System;
using Common;

namespace PowerScheme.Services
{
    using Model;
    using PowerSchemeServiceAPI;
    using RegistryManager.EventsArgs;
    using System.Reflection;
    using System.Windows.Forms;
    using static Utility.TrayIcon;

    public sealed partial class ViewService : IViewService
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

        public void Dispose()
        {
            _viewModel?.Dispose();
        }
    }
}
