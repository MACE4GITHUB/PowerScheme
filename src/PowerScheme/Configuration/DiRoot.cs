using System;
using System.ComponentModel;
using MessageForm;
using PowerScheme.Addins.IdleMonitoring;
using PowerScheme.Model;
using PowerScheme.Services;
using PowerScheme.Services.Settings;
using PowerSchemeServiceAPI;

namespace PowerScheme.Configuration;

/// <summary>
/// Provides access to application-wide services.
/// </summary>
public static class DiRoot
{
    private static readonly Lazy<IPowerSchemeService> _power = new(static () => new PowerSchemeService());
    private static readonly Lazy<IContainer> _container = new(static () => new Container());
    private static readonly Lazy<IIdleDetector> _idleDetector = new(static () => new Win32IdleDetector());
    private static readonly Lazy<IIdleMonitor> _idleMonitor = new(static () => new IdleMonitor(_idleDetector.Value));
    private static readonly Lazy<IIdleSettingsStore> _idleSettings = new(static () => new RegistryIdleSettingsStore(AppInfo.CompanyName, AppInfo.ProductName));
    private static readonly Lazy<IUpdateService> _updateService = new(static () => new UpdateService());
    private static readonly Lazy<LeftContextMenu> _leftContextMenu = new(static () => new LeftContextMenu(_container.Value, _power.Value));
    private static readonly Lazy<RightContextMenu> _rightContextMenu = new(static () => new RightContextMenu(_container.Value, _power.Value, _updateService.Value));
    private static readonly Lazy<IViewModel> _viewModel = new(static () => new ViewModel(_container.Value, _leftContextMenu.Value, _rightContextMenu.Value, _power.Value));
    private static readonly Lazy<IIdleMonitorService> _idleMonitorService = new(static () => new IdleMonitorService(_idleMonitor.Value, _power.Value, _idleSettings.Value));
    private static readonly Lazy<IViewService> _viewService = new(static () => new ViewService(_viewModel.Value, _updateService.Value, _idleMonitorService.Value));

    /// <summary>Gets the service of T.</summary>
    public static T GetService<T>() where T : class
    {
        var type = typeof(T);

        object? service =
            type == typeof(IPowerSchemeService) ? _power.Value :
            type == typeof(IViewModel) ? _viewModel.Value :
            type == typeof(IViewService) ? _viewService.Value :
            type == typeof(IUpdateService) ? _updateService.Value :
            type == typeof(IIdleMonitorService) ? _idleMonitorService.Value :
            type == typeof(IContainer) ? _container.Value :
            type == typeof(LeftContextMenu) ? _leftContextMenu.Value :
            type == typeof(RightContextMenu) ? _rightContextMenu.Value :
            type == typeof(IIdleDetector) ? _idleDetector.Value :
            type == typeof(IIdleMonitor) ? _idleMonitor.Value :
            type == typeof(IMainMessageBox) ? new MainMessageBox() :
            null;

        return service as T
               ?? throw new InvalidOperationException($"{type} is not registered in the service container");
    }
}
