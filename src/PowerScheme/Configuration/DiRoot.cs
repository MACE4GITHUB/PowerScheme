using System;
using MessageForm;
using Microsoft.Extensions.DependencyInjection;
using PowerScheme.Model;
using PowerSchemeServiceAPI;

namespace PowerScheme.Configuration;

/// <summary>
/// Provides access to application-wide services registered in the dependency injection container.
/// </summary>
/// <remarks>Use this class to retrieve singleton or transient services configured for the application's root
/// scope. All services are registered during static initialization and are available throughout the application's
/// lifetime. This class is intended for scenarios where global service access is required, such as in static contexts
/// or application entry points. For most application code, prefer constructor injection to access services.</remarks>
public static class DiRoot
{
    static DiRoot()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IPowerSchemeService, PowerSchemeService>();
        services.AddSingleton<IViewModel, ViewModel>();
        services.AddTransient<IMainMessageBox, MainMessageBox>();

        ServiceProvider = services.BuildServiceProvider();
    }

    private static IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Gets the service of T.
    /// </summary>
    public static T GetService<T>() where T : class
    {
        var service = ServiceProvider.GetService<T>();

        return service
               ?? throw new NullReferenceException($"{nameof(T)} not found in DI container");
    }
}
