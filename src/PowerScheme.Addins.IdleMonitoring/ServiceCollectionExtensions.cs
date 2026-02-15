using Microsoft.Extensions.DependencyInjection;

namespace PowerScheme.Addins.IdleMonitoring;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdleMonitoring(this IServiceCollection services)
    {
        services.AddSingleton<IIdleDetector, Win32IdleDetector>();
        services.AddSingleton<IIdleMonitor, IdleMonitor>();

        return services;
    }
}
