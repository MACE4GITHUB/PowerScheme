using System;
using System.Collections.Generic;
using PowerSchemeServiceAPI.EventsArgs;
using PowerSchemeServiceAPI.Model;
using PowerSchemeServiceAPI.Settings;

namespace PowerSchemeServiceAPI;

public interface IPowerSchemeService: IPowerSchemeDisplayService, IPowerSchemeSleepService
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

public interface IPowerSchemeDisplayService
{
    void SetAllPowerSchemesIdleDisplay(int value);

    void SetIdleDisplay(Guid guid, int value);

    PowerSchemeDcAcValues GetIdleDisplay(Guid guid);

    void SetAllPowerSchemesIdleLockDisplay(int value);

    void SetIdleLockDisplay(Guid guid, int value);

    PowerSchemeDcAcValues GetIdleLockDisplay(Guid guid);
}

public interface IPowerSchemeSleepService
{
    void SetAllPowerSchemesIdleSleep(int value);

    void SetIdleSleep(Guid guid, int value);

    PowerSchemeDcAcValues GetIdleSleep(Guid guid);

    void SetAllPowerSchemesIdleHibernate(int value);

    void SetIdleHibernate(Guid guid, int value);

    PowerSchemeDcAcValues GetIdleHibernate(Guid guid);
}
