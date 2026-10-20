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
    private static readonly MethodInfo _showContextMenu = typeof(NotifyIcon)
        .GetMethod(SHOW_CONTEXT_MENU, BindingFlags.Instance | BindingFlags.NonPublic)
        ?? throw new MissingMethodException(typeof(NotifyIcon).FullName, SHOW_CONTEXT_MENU);
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

    private void ActivePowerSchemeOnChanged(object? sender, RegistryChangedEventArgs e)
    {
        UpdateIcon();
    }

    public void UpdateIcon()
    {
        var activePowerScheme = _viewModel.Power.ActivePowerScheme;
        var image = activePowerScheme.Picture;
        var icon = image.GetIcon();

        _viewModel.UpdateIcon(icon);
        _viewModel.NotifyIcon.Text = activePowerScheme.Name;
    }

    private void NotifyIcon_MouseClick(object? sender, MouseEventArgs e) =>
        ShowContextMenu(_viewModel, e.Button, static icon => _showContextMenu.Invoke(icon, null));

    internal static void ShowContextMenu(
        IViewModel viewModel,
        MouseButtons button,
        Action<NotifyIcon> showMenu)
    {
        if (button is not (MouseButtons.Left or MouseButtons.Right))
        {
            return;
        }

        var menu = button == MouseButtons.Right
            ? viewModel.ContextRightMenu
            : viewModel.ContextLeftMenu;

        menu.UpdateMenu();

        var notifyIcon = viewModel.NotifyIcon;
        notifyIcon.ContextMenuStrip = menu;

        try
        {
            showMenu(notifyIcon);
        }
        finally
        {
            notifyIcon.ContextMenuStrip = null;
        }
    }

    protected override void Dispose(bool disposing)
    {
        Stop();
        _viewModel.Dispose();
        base.Dispose(disposing);
    }
}
