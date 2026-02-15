using System;
using System.ComponentModel;
using MessageForm;
using Microsoft.Extensions.DependencyInjection;
using PowerScheme.Addins.IdleMonitoring;
using PowerScheme.Model;
using PowerScheme.Services;
using PowerSchemeServiceAPI;

namespace PowerScheme.Configuration;

/// <summary>
/// Provides access to application-wide services registered
/// in the dependency injection container.
/// </summary>
public static class DiRoot
{
    static DiRoot()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IPowerSchemeService, PowerSchemeService>()
            .AddSingleton<IViewModel, ViewModel>()
            .AddSingleton<IViewService, ViewService>()
            .AddSingleton<IUpdateService, UpdateService>()
            .AddSingleton<IContainer, Container>()
            .AddSingleton<LeftContextMenu>()
            .AddSingleton<RightContextMenu>()
            .AddTransient<IMainMessageBox, MainMessageBox>()
            .AddIdleMonitoring();

        ServiceProvider = services.BuildServiceProvider();
    }

    private static IServiceProvider ServiceProvider { get; }

    /// <summary>Gets the service of T.</summary>
    public static T GetService<T>() where T : class
    {
        var service = ServiceProvider.GetService<T>();

        return service
               ?? throw new InvalidOperationException($"{nameof(T)} not found in DI container");
    }
}
