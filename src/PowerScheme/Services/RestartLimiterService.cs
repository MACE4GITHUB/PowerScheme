using PowerScheme.Model;
using RegistryManager;

namespace PowerScheme.Services;

internal static class RestartLimiterService
{
    private const int MAX_RESTARTS = 2;

    /// <summary>
    /// Determines whether the application can be safely restarted based on the configured maximum number of allowed
    /// restarts.
    /// </summary>
    /// <remarks>This method tracks the number of restart attempts using persistent storage. If the maximum
    /// number of restarts is reached, the count is reset and the method returns false. If an error occurs while
    /// accessing the restart count, the method returns false.</remarks>
    /// <returns>true if the application has not exceeded the maximum number of allowed restarts; otherwise, false.</returns>
    public static bool CanRestart()
    {
        try
        {
            var count = RegistryService.RestartCount(AppInfo.CompanyName, AppInfo.ProductName);

            if (count >= MAX_RESTARTS)
            {
                RegistryService.SetRestartCount(AppInfo.CompanyName, AppInfo.ProductName, 0);

                return false;
            }

            RegistryService.SetRestartCount(AppInfo.CompanyName, AppInfo.ProductName, count + 1);

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Resets the application's restart count to its initial state.
    /// </summary>
    /// <remarks>This method removes any persisted restart count information for the current application. It
    /// is safe to call this method multiple times; if no restart count exists, the method has no effect.</remarks>
    public static void Reset()
    {
        try
        {
            RegistryService.DeleteRestartCount(AppInfo.CompanyName, AppInfo.ProductName);
        }
        catch
        {
            // Do nothing
        }
    }
}
