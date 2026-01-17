using MessageForm;
using Microsoft.Extensions.DependencyInjection;
using PowerScheme.Model;
using PowerSchemeServiceAPI;

namespace PowerScheme.Configuration;

/// <summary>
/// Determines methods for application configuration.
/// </summary>
internal static class DiConfigurator
{
    /// <summary>
    /// Creates application configuration.
    /// </summary>
    public static IServiceCollection Configure(IServiceCollection? services = null)
    {
        services ??= new ServiceCollection();
        services.AddSingleton<IPowerSchemeService, PowerSchemeService>();
        services.AddSingleton<IViewModel, ViewModel>();
        services.AddTransient<IMainMessageBox, MainMessageBox>();

        return services;
    }
}
