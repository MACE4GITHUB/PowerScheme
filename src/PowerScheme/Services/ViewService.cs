using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Common.Paths;
using PowerScheme.Model;
using PowerScheme.Timers;
using RegistryManager;
using RegistryManager.Common;
using RegistryManager.Dpi;
using RegistryManager.EventsArgs;
using static PowerScheme.Utility.ImageExtensions;

namespace PowerScheme.Services;

internal sealed class ViewService : ApplicationContext, IViewService
{
    private const string SHOW_CONTEXT_MENU = "ShowContextMenu";
    private readonly IViewModel _viewModel;
    private readonly UpdateTimer _updateTimer;
    private readonly DpiWatchers _dpiWatchers;
    private readonly IIdleMonitorService _idleMonitorService;

    public ViewService(
        IViewModel viewModel,
        IUpdateService updateService,
        IIdleMonitorService idleMonitorService)
    {
        _viewModel = viewModel;
        _updateTimer = new UpdateTimer(updateService);

        _dpiWatchers = new DpiWatchers();

        _idleMonitorService = idleMonitorService;

        Start();
    }

    public void Start()
    {
        _viewModel.Power.Watchers.ActivePowerScheme.Changed += ActivePowerSchemeOnChanged;
        _viewModel.Power.Watchers.PowerSchemes.Changed += PowerSchemesOnChanged;

        _viewModel.NotifyIcon.MouseClick += NotifyIcon_MouseClick;

        _dpiWatchers.Changed += DpiWatchers_Changed;

        _viewModel.NotifyIcon.Visible = true;

        UpdateIcon();

        _viewModel.Power.Watchers.Start();
        _viewModel.BuildAllMenu();

        _updateTimer.Start();
        _dpiWatchers.Start();
        _idleMonitorService.Start();
    }

    private static void PowerSchemesOnChanged(object sender, RegistryChangedEventArgs e)
    {
        if (e is not { RegChangeNotifyFilter: RegChangeNotifyFilter.Key, ChangeType: ChangeType.Deleted })
        {
            return;
        }

        var idlePowerSchemeGuid = RegistryService.GetIdleMonitoring(AppInfo.CompanyName, AppInfo.ProductName);

        if (e.SubKeys == null || e.SubKeys.Length == 0)
        {
            return;
        }

        List<Guid> subKeys = [..e.SubKeys
            .Select(x => Guid.TryParse(x, out var guid) ? guid : Guid.Empty)
            .Where(x=> x != Guid.Empty)];

        if (subKeys.Contains(idlePowerSchemeGuid))
        {
            RegistryService.SetIdlePowerScheme(AppInfo.CompanyName, AppInfo.ProductName, Guid.Empty);
        }
    }

    public void Stop()
    {
        _updateTimer.Stop();

        _viewModel.Power.Watchers.ActivePowerScheme.Changed -= ActivePowerSchemeOnChanged;

        _viewModel.Power.Watchers.Stop();
        _viewModel.ClearAllMenu();

        _dpiWatchers.Stop();
        _dpiWatchers.Changed -= DpiWatchers_Changed;

        _idleMonitorService.Stop();

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

    private void ActivePowerSchemeOnChanged(object? sender, RegistryChangedEventArgs e)
    {
        UpdateIcon();
    }

    private void UpdateIcon()
    {
        var activePowerScheme = _viewModel.Power.ActivePowerScheme;
        var image = activePowerScheme.Picture;
        var icon = image.GetIcon();

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

    protected override void Dispose(bool disposing)
    {
        Stop();
        _viewModel.Dispose();
        base.Dispose(disposing);
    }
}
