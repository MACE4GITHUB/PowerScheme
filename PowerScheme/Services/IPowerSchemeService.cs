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

        Watchers Watchers { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="powerScheme"></param>
        /// <param name="isForce">Need to apply new AC & DC values</param>
        void SetActivePowerScheme(IPowerScheme powerScheme, bool isForce = false);

        bool IsNeedAdminAccessForChangePowerScheme { get; }

        bool ExistsMobilePlatformRole { get; }

        bool ExistsHibernate { get; }

        bool ExistsSleep { get; }

        bool CanCreateExtremePowerScheme { get; }

        bool ExistsStablePowerScheme { get; }
        bool ExistsMediaPowerScheme { get; }
        bool ExistsSimplePowerScheme { get; }
        bool ExistsExtremePowerScheme { get; }
        bool ExistsAllTypicalScheme { get; }

        void RestoreDefaultPowerSchemes();

        void DeleteAllTypicalScheme();

        void CreateTypicalSchemes();

        void CreateStablePowerScheme();

        void DeleteStablePowerScheme();

        void CreateMediaPowerScheme();

        void DeleteMediaPowerScheme();

        void CreateSimplePowerScheme();

        void DeleteSimplePowerScheme();

        void CreateExtremePowerScheme();

        void DeleteExtremePowerScheme();

        void SetLid(int value);

        event EventHandler<PowerSchemeEventArgs> ActivePowerSchemeChanged;

    }
}