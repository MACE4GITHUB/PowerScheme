using RegistryManager.Common;
using static RegistryManager.Dpi.DpiRegistryParams;

namespace RegistryManager.Dpi;

public static class DpiRegistryWatcher
{
    public static RegistryWatcher<string> ScaleFactorsRegistryWatcher()
        => new(GetScaleFactors)
        {
            RegChangeNotifyFilter = RegChangeNotifyFilter.Key
        };

    public static RegistryWatcher<int> MonitorWatcher(RegistryParam registryParam)
        => new(registryParam)
        {
            RegChangeNotifyFilter = RegChangeNotifyFilter.Value
        };
}
