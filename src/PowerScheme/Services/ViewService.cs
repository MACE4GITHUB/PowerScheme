using System.Reflection;
using System.Windows.Forms;
using PowerScheme.EventArguments;
using PowerScheme.Model;
using PowerScheme.Timers;
using RegistryManager.EventsArgs;
using static PowerScheme.Utility.TrayIcon;

namespace PowerScheme.Services;

public sealed class ViewService : ApplicationContext, IViewService
{
    private const string SHOW_CONTEXT_MENU = "ShowContextMenu";
    private readonly IViewModel _viewModel;
    private readonly UpdateTimer _updateTimer;

    public ViewService(
        IViewModel viewModel,
        IUpdateService updateService)
    {
        _viewModel = viewModel;
        _updateTimer = new UpdateTimer(updateService);
        _updateTimer.NotifyUpdate += NotifyAppUpdate;
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

        _updateTimer.Start();
    }

    public void Stop()
    {
        _viewModel.Power.Watchers.ActivePowerScheme.Changed -= ChangedActivePowerScheme;

        _viewModel.NotifyIcon.MouseClick -= NotifyIcon_MouseClick;
        _viewModel.NotifyIcon.Visible = false;

        _viewModel.RemoveIcon();

        _viewModel.Power.Watchers.Stop();
        _viewModel.ClearAllMenu();
    }

    private void ChangedActivePowerScheme(object? sender, RegistryChangedEventArgs e)
    {
        UpdateIcon();
    }

    private void UpdateIcon()
    {
        var activePowerScheme = _viewModel.Power.ActivePowerScheme;
        var image = activePowerScheme.Picture;
        var icon = GetIcon(image);

        _viewModel.UpdateIcon(icon);
        _viewModel.NotifyIcon.Text = activePowerScheme.Name;
    }

    private void NotifyIcon_MouseClick(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            _viewModel.ContextRightMenu.UpdateMenu();
            _viewModel.NotifyIcon.ContextMenuStrip = _viewModel.ContextRightMenu;
        }
        else
        {
            _viewModel.ContextLeftMenu.UpdateMenu();
            _viewModel.NotifyIcon.ContextMenuStrip = _viewModel.ContextLeftMenu;
        }

        var mi = typeof(NotifyIcon)
            .GetMethod(SHOW_CONTEXT_MENU, BindingFlags.Instance | BindingFlags.NonPublic);
        mi?.Invoke(_viewModel.NotifyIcon, null);

        _viewModel.NotifyIcon.ContextMenuStrip = null;
    }

    private void NotifyAppUpdate(object sender, UpdateEventArgs e)
    {
        var releaseInfo = e.ReleaseInfo;

        if (releaseInfo.NewVersionAvailable)
        {
            ((RightContextMenu)_viewModel.ContextRightMenu)
                .SetNewVersion(releaseInfo);
        }
    }

    protected override void Dispose(bool disposing)
    {
        Stop();
        _viewModel?.Dispose();
        base.Dispose(disposing);
    }
}
