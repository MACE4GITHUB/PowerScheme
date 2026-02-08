using System;
using System.ComponentModel;
using MessageForm;
using Microsoft.Extensions.DependencyInjection;
using PowerScheme.Model;
using PowerScheme.Services;
using PowerSchemeServiceAPI;

namespace PowerScheme.Configuration;

/// <summary>
/// Provides access to application-wide services registered in the dependency injection container.
/// </summary>
public static class DiRoot
{
    static DiRoot()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IPowerSchemeService, PowerSchemeService>();
        services.AddSingleton<IViewModel, ViewModel>();
        services.AddSingleton<IViewService, ViewService>();
        services.AddSingleton<IUpdateService, UpdateService>();
        services.AddSingleton<IContainer, Container>();
        services.AddSingleton<LeftContextMenu>();
        services.AddSingleton<RightContextMenu>();
        services.AddTransient<IMainMessageBox, MainMessageBox>();

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
