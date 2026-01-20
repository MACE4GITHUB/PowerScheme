using System;
using System.Collections.Generic;
using PowerSchemeServiceAPI.EventsArgs;
using PowerSchemeServiceAPI.Model;

namespace PowerSchemeServiceAPI;

public interface IPowerSchemeService
{
    IPowerScheme ActivePowerScheme { get; }
    IEnumerable<IPowerScheme> DefaultPowerSchemes { get; }
    IEnumerable<IPowerScheme> TypicalPowerSchemes { get; }
    IEnumerable<IPowerScheme> UserPowerSchemes { get; }

    Watchers Watchers { get; }

    /// <summary>
    /// Set the Active Power Scheme.
    /// <para>isForce - Need to apply new AC & DC values</para>
    /// </summary>
    /// <param name="powerScheme"></param>
    /// <param name="isForce">Need to apply new AC & DC values</param>
    void SetActivePowerScheme(IPowerScheme powerScheme, bool isForce = false);

    bool IsNeedAdminAccessForChangePowerScheme { get; }

    bool ExistsMobilePlatformRole { get; }

    bool ExistsHibernate { get; }

    bool ExistsSleep { get; }

    bool ExistsAllTypicalScheme { get; }

    void RestoreDefaultPowerSchemes();

    void DeleteAllTypicalScheme();

    void CreateTypicalSchemes();

    void SetLid(int value);

    string TextActionToggle(StatePowerScheme statePowerScheme);

    StatePowerScheme? StatePowerSchemeToggle(StatePowerScheme statePowerScheme);

    void ActionPowerScheme(StatePowerScheme statePowerScheme);

    event EventHandler<PowerSchemeEventArgs> ActivePowerSchemeChanged;
}
