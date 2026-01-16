namespace PowerScheme.Configuration;

using System;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Creates dependency injection root.
/// </summary>
public static class DiRoot
{
    public static IServiceProvider ServiceProvider { get; private set; }

    /// <summary>
    /// Configures services and builds service provider.
    /// </summary>
    public static void ConfigureServices(IServiceCollection services) =>
        ServiceProvider = services?.BuildServiceProvider();

    /// <summary>
    /// Gets the service of T.
    /// </summary>
    public static T GetService<T>() where T : class =>
        (T)ServiceProvider.GetService(typeof(T));
}