using System;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="powerScheme"></param>
        /// <param name="isForce">Need to apply new AC & DC values</param>
        void SetActivePowerScheme(IPowerScheme powerScheme, bool isForce = false);

        bool IsNeedAdminAccessForChangePowerScheme { get; }

        bool IsMobilePlatformRole { get; }

        bool IsHibernate { get; }

        bool IsSleep { get; }

        void RestoreDefaultPowerSchemes();

        void DeleteTypicalScheme();

        void CreateTypicalSchemes();

        void CreateStablePowerScheme();

        void CreateMediaPowerScheme();

        void CreateSimplePowerScheme();

        void SetLid(int value);

        event EventHandler<PowerSchemeEventArgs> ActivePowerSchemeChanged;

    }
}