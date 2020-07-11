﻿using System;
using System.Collections.Generic;
using PowerScheme.EventsArgs;
using PowerScheme.Model;

namespace PowerScheme.Services
{
    public interface IPowerSchemeService
    {
        IPowerScheme ActivePowerScheme { get; }
        IEnumerable<IPowerScheme> DefaultPowerSchemes { get; }
        IEnumerable<IPowerScheme> UserPowerSchemes { get; }

        IPowerScheme FirstAnyPowerScheme { get; }

        Watchers Watchers { get; }
        void SetActivePowerScheme(IPowerScheme powerScheme);

        bool IsNeedAdminAccessForChangePowerScheme();

        void RestoreDefaultPowerSchemes();

        bool IsMobilePlatformRole();

        bool IsHibernate();

        bool IsSleep();

        void DeleteTypicalScheme();

        void CreateStablePowerScheme(string name, string description = null);

        void CreateMediaPowerScheme(string name, string description = null);

        void CreateSimplePowerScheme(string name, string description = null);

        void SetLid(int value);

        event EventHandler<PowerSchemeEventArgs> ActivePowerSchemeChanged;

    }
}