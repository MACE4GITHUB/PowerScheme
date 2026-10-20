using System;
using System.Collections.Generic;
using PowerSchemeServiceAPI;
using PowerSchemeServiceAPI.EventsArgs;
using PowerSchemeServiceAPI.Model;
using PowerSchemeServiceAPI.Settings;

namespace PowerScheme.Tests.TestDoubles;

internal sealed class TestPowerService(IPowerScheme initialScheme) : IPowerSchemeService
{
    public event EventHandler<PowerSchemeEventArgs> ActivePowerSchemeChanged = delegate { };
    public IPowerScheme ActivePowerScheme { get; private set; } = initialScheme;
    public IEnumerable<IPowerScheme> Schemes { get; set; } = [];
    public IEnumerable<IPowerScheme> NativeSchemes { get; set; } = [];
    public IEnumerable<IPowerScheme> PowerSchemes => Schemes;
    public IEnumerable<IPowerScheme> PowerProfPowerSchemes => NativeSchemes;
    public List<IPowerScheme> Activations { get; } = [];
    public List<(Guid Source, Guid Destination)> BrightnessCopies { get; } = [];
    public List<string> Operations { get; } = [];
    public Action<IPowerScheme>? BeforeActivation { get; set; }
    public Action<IPowerScheme>? AfterActivation { get; set; }
    public Action? BeforeBrightnessCopy { get; set; }
    public Dictionary<(string Setting, Guid Scheme), PowerSchemeDcAcValues> Values { get; } = [];
    public List<(string Setting, Guid Scheme)> Reads { get; } = [];
    public List<(string Setting, Guid? Scheme, int Value)> Writes { get; } = [];
    public int RenameCount { get; private set; }

    public void SetActivePowerScheme(IPowerScheme newPowerScheme, bool isForce = false)
    {
        BeforeActivation?.Invoke(newPowerScheme);
        var previous = ActivePowerScheme;
        ActivePowerScheme = newPowerScheme;
        Activations.Add(newPowerScheme);
        Operations.Add("activate");
        if (!isForce)
        {
            RaiseSchemeChanged(previous, newPowerScheme);
        }
        AfterActivation?.Invoke(newPowerScheme);
    }

    public void RaiseSchemeChanged(IPowerScheme previous, IPowerScheme active) =>
        ActivePowerSchemeChanged.Invoke(this, new PowerSchemeEventArgs(previous, active));

    public void CopyBrightness(Guid sourcePowerScheme, Guid destinationPowerScheme)
    {
        BeforeBrightnessCopy?.Invoke();
        BrightnessCopies.Add((sourcePowerScheme, destinationPowerScheme));
        Operations.Add("brightness");
    }

    private PowerSchemeDcAcValues ReadValues(string setting, Guid guid)
    {
        Reads.Add((setting, guid));
        return Values.TryGetValue((setting, guid), out var values) ? values : new PowerSchemeDcAcValues(0, 0);
    }

    public void SetAllPowerSchemesIdleDisplay(int value) => Writes.Add(("display", null, value));
    public void SetIdleDisplay(Guid guid, int value) => Writes.Add(("display", guid, value));
    public PowerSchemeDcAcValues GetIdleDisplay(Guid guid) => ReadValues("display", guid);
    public void SetAllPowerSchemesIdleLockDisplay(int value) => Writes.Add(("lock", null, value));
    public void SetIdleLockDisplay(Guid guid, int value) => Writes.Add(("lock", guid, value));
    public PowerSchemeDcAcValues GetIdleLockDisplay(Guid guid) => ReadValues("lock", guid);
    public void SetAllPowerSchemesIdleSleep(int value) => Writes.Add(("sleep", null, value));
    public void SetIdleSleep(Guid guid, int value) => Writes.Add(("sleep", guid, value));
    public PowerSchemeDcAcValues GetIdleSleep(Guid guid) => ReadValues("sleep", guid);
    public void SetAllPowerSchemesIdleHibernate(int value) => Writes.Add(("hibernate", null, value));
    public void SetIdleHibernate(Guid guid, int value) => Writes.Add(("hibernate", guid, value));
    public PowerSchemeDcAcValues GetIdleHibernate(Guid guid) => ReadValues("hibernate", guid);

    public IEnumerable<IPowerScheme> TypicalPowerSchemesWithDeleted => throw new NotSupportedException();
    public IEnumerable<IPowerScheme> TypicalPowerSchemesWithoutDeleted => throw new NotSupportedException();
    public IEnumerable<IPowerScheme> UserPowerSchemes => throw new NotSupportedException();
    public IEnumerable<IPowerScheme> CustomPowerSchemes => throw new NotSupportedException();
    public Watchers Watchers => throw new NotSupportedException();
    public bool IsNeedAdminAccessForChangePowerScheme => throw new NotSupportedException();
    public bool ExistsMobilePlatformRole => throw new NotSupportedException();
    public bool ExistsHibernate => throw new NotSupportedException();
    public bool ExistsSleep => throw new NotSupportedException();
    public bool ExistsAllTypicalScheme => throw new NotSupportedException();
    public void RestoreDefaultPowerSchemes() => throw new NotSupportedException();
    public void DeleteAllTypicalScheme() => throw new NotSupportedException();
    public void CreateTypicalSchemes() => throw new NotSupportedException();
    public void RenameTypicalSchemes() => RenameCount++;
    public void SetLid(int value) => throw new NotSupportedException();
    public string TextActionToggle(StatePowerScheme statePowerScheme) => throw new NotSupportedException();
    public StatePowerScheme? StatePowerSchemeToggle(StatePowerScheme statePowerScheme) => throw new NotSupportedException();
    public void ActionPowerScheme(StatePowerScheme statePowerScheme) => throw new NotSupportedException();
    public void DeletePowerScheme(StatePowerScheme statePowerScheme) => throw new NotSupportedException();
}
