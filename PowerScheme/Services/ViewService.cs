using System.Reflection;

namespace PowerScheme.Services
{
    using Model;
    using RegistryManager.EventsArgs;
    using System.Windows.Forms;
    using static Utility.TrayIcon;

    public sealed class ViewService : ApplicationContext, IViewService
    {
        private const string SHOW_CONTEXT_MENU = "ShowContextMenu";
        private readonly IViewModel _viewModel;

        public ViewService(IViewModel viewModel)
        {
            _viewModel = viewModel;
            Start();
        }

        public void Start()
        {
            _viewModel.Power.Watchers.ActivePowerScheme.Changed += ChangedActivePowerScheme;

            _viewModel.NotifyIcon.MouseClick += NotifyIcon_MouseClick;

            _viewModel.NotifyIcon.Visible = true;

            UpdateIcon();

            _viewModel.Power.Watchers.Start();
            _viewModel.BuildAllMenu();
        }

        public void Stop()
        {
            _viewModel.Power.Watchers.ActivePowerScheme.Changed -= ChangedActivePowerScheme;

            _viewModel.NotifyIcon.MouseClick -= NotifyIcon_MouseClick;
            _viewModel.NotifyIcon.Visible = false;

            _viewModel.Power.Watchers.Stop();
            _viewModel.ClearAllMenu();
        }

        private void ChangedActivePowerScheme(object sender, RegistryChangedEventArgs e)
        {
            UpdateIcon();
        }

        private void UpdateIcon()
        {
            var activePowerScheme = _viewModel.Power.ActivePowerScheme;
            var image = activePowerScheme.Picture;
            var icon = GetIcon(image);

            _viewModel.NotifyIcon.Icon = icon;
            _viewModel.NotifyIcon.Text = activePowerScheme.Name;
        }

        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            _viewModel.NotifyIcon.ContextMenuStrip =
            e.Button == MouseButtons.Right ?
                _viewModel.ContextRightMenu :
                _viewModel.ContextLeftMenu;

            if (e.Button == MouseButtons.Right)
            {
                _viewModel.ContextRightMenu.UpdateMenu();
            }
            else
            {
                _viewModel.ContextLeftMenu.UpdateMenu();
            }

            var mi = typeof(NotifyIcon)
                .GetMethod(SHOW_CONTEXT_MENU, BindingFlags.Instance | BindingFlags.NonPublic);
            mi?.Invoke(_viewModel.NotifyIcon, null);

            _viewModel.NotifyIcon.ContextMenuStrip = null;
        }

        protected override void Dispose(bool disposing)
        {
            Stop();
            _viewModel?.Dispose();
            base.Dispose(disposing);
        }
    }
}
