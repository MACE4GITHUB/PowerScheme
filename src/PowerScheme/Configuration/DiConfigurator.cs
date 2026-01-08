namespace PowerScheme.Configuration;

using MessageForm;
using Microsoft.Extensions.DependencyInjection;
using Model;
using PowerSchemeServiceAPI;

/// <summary>
/// Determines methods for application configuration.
/// </summary>
internal sealed class DiConfigurator
{
    /// <summary>
    /// Creates application configuration.
    /// </summary>
    public IServiceCollection Configure(IServiceCollection services = null)
    {
        services ??= new ServiceCollection();
        services.AddSingleton<IPowerSchemeService, PowerSchemeService>();
        services.AddSingleton<IViewModel, ViewModel>();
        services.AddTransient<IMainMessageBox, MainMessageBox>();
        
        return services;
    }
}