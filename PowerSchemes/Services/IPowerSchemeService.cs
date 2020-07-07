using System;
using System.Collections.Generic;
using PowerSchemes.EventsArgs;
using PowerSchemes.Model;

namespace PowerSchemes.Services
{
    public interface IPowerSchemeService
    {
        IPowerScheme ActivePowerScheme { get; }
        IEnumerable<IPowerScheme> DefaultPowerSchemes { get; }
        IEnumerable<IPowerScheme> UserPowerSchemes { get; }
        Watchers Watchers { get; }
        void SetActivePowerScheme(IPowerScheme powerScheme);

        void RestoreDefaultPowerSchemes();

        bool IsMobilePlatformRole();

        void DeleteTypicalScheme();

        void CreateStablePowerScheme(string name, string description = null);

        void CreateMediaPowerScheme(string name, string description = null);

        void CreateSimplePowerScheme(string name, string description = null);

        event EventHandler<PowerSchemeEventArgs> ActivePowerSchemeChanged;

    }
}