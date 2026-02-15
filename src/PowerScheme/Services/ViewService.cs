using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Common.Paths;
using PowerScheme.Addins.IdleMonitoring;
using PowerScheme.EventArguments;
using PowerScheme.Model;
using PowerScheme.Timers;
using RegistryManager;
using RegistryManager.Dpi;
using RegistryManager.EventsArgs;
using static PowerScheme.Addins.IdleMonitoring.Constants;
using static PowerScheme.Utility.TrayIcon;

namespace PowerScheme.Services;

public sealed class ViewService : ApplicationContext, IViewService
{
    private const string SHOW_CONTEXT_MENU = "ShowContextMenu";
    private readonly IViewModel _viewModel;
    private readonly IIdleMonitor _idleMonitor;
    private readonly UpdateTimer _updateTimer;
    private readonly DpiWatchers _dpiWatchers;

    public ViewService(
        IViewModel viewModel,
        IUpdateService updateService,
        IIdleMonitor idleMonitor)
    {
        _viewModel = viewModel;
        _updateTimer = new UpdateTimer(updateService);
        _updateTimer.NotifyUpdate += NotifyAppUpdate;

        _dpiWatchers = new DpiWatchers();
        _dpiWatchers.Changed += DpiWatchers_Changed;

        _idleMonitor = idleMonitor;
        _idleMonitor.OnIdle += IdleMonitor_OnIdle;
        _idleMonitor.OnActive += IdleMonitor_OnActive;

        Start();
    }

    private void IdleMonitor_OnActive(object sender, EventArgs e)
    {
        var previousPowerScheme = _viewModel.Power
            .DefaultPowerSchemes
            .First(x => x.Guid == new Guid("8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c"));

        _viewModel.Power.SetActivePowerScheme(previousPowerScheme);
    }

    private void IdleMonitor_OnIdle(object sender, EventArgs e)
    {
        var idlePowerSchemeGuid = GetIdlePowerSchemeGuid();

        var idlePowerScheme = _viewModel.Power
            .UserPowerSchemes
            .FirstOrDefault(x => x.Guid == idlePowerSchemeGuid) ??
             _viewModel.Power
                .DefaultPowerSchemes
                .FirstOrDefault(x => x.Guid == idlePowerSchemeGuid) ??
            _viewModel.Power
                .DefaultPowerSchemes
                .Last(x => !x.IsMaxPerformance && x.IsNative);

        _viewModel.Power.SetActivePowerScheme(idlePowerScheme);
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
        _dpiWatchers.Start();

        var idlePowerSchemeGuid = GetIdlePowerSchemeGuid();
        if (idlePowerSchemeGuid != Guid.Empty)
        {
            _idleMonitor.StartAsync(TimeSpan.FromSeconds(IDLE_THRESHOLD_IN_SECONDS));
        }
    }

    public void Stop()
    {
        _updateTimer.Stop();
        _idleMonitor.Stop();

        _viewModel.Power.Watchers.ActivePowerScheme.Changed -= ChangedActivePowerScheme;

        _viewModel.Power.Watchers.Stop();
        _viewModel.ClearAllMenu();

        _dpiWatchers.Stop();
        _dpiWatchers.Changed -= DpiWatchers_Changed;

        _viewModel.NotifyIcon.MouseClick -= NotifyIcon_MouseClick;
        _viewModel.NotifyIcon.Visible = false;

        _viewModel.RemoveIcon();
    }

    private static void DpiWatchers_Changed(object sender, RegistryChangedEventArgs e)
    {
        Program.OnceAppMutex?.Dispose();

        System.Diagnostics.Process.Start(Default.ApplicationFileName);
        Application.Exit();
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

    private static Guid GetIdlePowerSchemeGuid() =>
        RegistryService.GetIdleMonitoring(AppInfo.CompanyName, AppInfo.ProductName);

    protected override void Dispose(bool disposing)
    {
        Stop();
        _viewModel?.Dispose();
        base.Dispose(disposing);
    }
}
