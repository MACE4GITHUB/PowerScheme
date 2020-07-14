using System;
using RegistryManager;

namespace PowerSchemeServiceAPI.Model
{
    public class Watchers
    {
        public IRegistryWatcher ActivePowerScheme
        { get; } = RegistryService.ActivePowerSchemeRegistryWatcher();

        public IRegistryWatcher PowerSchemes
        { get; } = RegistryService.PowerSchemesRegistryWatcher();

        public void Start()
        {
            if (!ActivePowerScheme.IsMonitoring) ActivePowerScheme.Start();
            if (!PowerSchemes.IsMonitoring) PowerSchemes.Start();
        }

        public void Stop()
        {
            if (ActivePowerScheme.IsMonitoring) ActivePowerScheme.Stop();
            if (PowerSchemes.IsMonitoring) PowerSchemes.Stop();
        }

        public void RaiseActionWithoutWatchers(Action action)
        {
            var tempW1 = ActivePowerScheme.IsMonitoring;
            var tempW2 = PowerSchemes.IsMonitoring;

            Stop();
            action();

            if (tempW1) ActivePowerScheme.Start();
            if (tempW2) PowerSchemes.Start();
        }

    }
}
