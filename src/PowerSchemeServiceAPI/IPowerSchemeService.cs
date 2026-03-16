using System;
using System.Collections.Generic;
using PowerSchemeServiceAPI.EventsArgs;
using PowerSchemeServiceAPI.Model;

namespace PowerSchemeServiceAPI;

public interface IPowerSchemeService
{
    IPowerScheme ActivePowerScheme { get; }
    IEnumerable<IPowerScheme> PowerProfPowerSchemes { get; }
    IEnumerable<IPowerScheme> TypicalPowerSchemesWithDeleted { get; }
    IEnumerable<IPowerScheme> TypicalPowerSchemesWithoutDeleted { get; }
    IEnumerable<IPowerScheme> UserPowerSchemes { get; }
    IEnumerable<IPowerScheme> CustomPowerSchemes { get; }
    IEnumerable<IPowerScheme> PowerSchemes { get; }

    Watchers Watchers { get; }

    /// <summary>
    /// Set the Active Power Scheme.
    /// <para>isForce - Need to apply new AC & DC values</para>
    /// </summary>
    /// <param name="newPowerScheme"></param>
    /// <param name="isForce">Need to apply new AC & DC values</param>
    void SetActivePowerScheme(IPowerScheme newPowerScheme, bool isForce = false);

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

    void DeletePowerScheme(StatePowerScheme statePowerScheme);

    event EventHandler<PowerSchemeEventArgs> ActivePowerSchemeChanged;

    void CopyBrightness(Guid sourcePowerScheme, Guid destinationPowerScheme);
}
