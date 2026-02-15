using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Languages;
using PowerManagerAPI;
using PowerSchemeServiceAPI.EventsArgs;
using PowerSchemeServiceAPI.Model;
using PowerSchemeServiceAPI.Settings;
using RegistryManager;
using static PowerSchemeServiceAPI.SettingSchemeLookup;

namespace PowerSchemeServiceAPI;

public class PowerSchemeService : IPowerSchemeService
{
    public IEnumerable<IPowerScheme> DefaultPowerSchemes
        => SettingSchemes
            .Where(p => p.Value.IsNative && p.Value.IsVisible)
            .Select(p => p.Value);

    public IEnumerable<IPowerScheme> TypicalPowerSchemes
    {
        get
        {
            if (CanCreateExtremePowerScheme)
            {
                return SettingSchemes
                    .Where(p => !p.Value.IsNative && p.Value.IsVisible)
                    .Select(p => p.Value);
            }

            return SettingSchemes
                .Where(p => !p.Value.IsNative && p.Value.IsVisible && !p.Value.IsMaxPerformance)
                .Select(p => p.Value);
        }
    }

    public IEnumerable<IPowerScheme> UserPowerSchemes
        => RegistryService.UserPowerSchemes.Select(NewPowerScheme);

    public IEnumerable<IPowerScheme> PowerSchemes
        => DefaultPowerSchemes.Union(UserPowerSchemes);

    public IPowerScheme ActivePowerScheme
    {
        get
        {
            var guidActivePlan = PowerManager.GetActivePlan();
            return PowerSchemes.FirstOrDefault(p => p.Guid == guidActivePlan);
        }
    }

    public bool IsNeedAdminAccessForChangePowerScheme
    {
        get
        {
            var result = false;

            try
            {
                PowerManager.SetActivePlan(ActivePowerScheme.Guid);
            }
            catch (Exception)
            {
                result = true;
            }

            return result;
        }
    }

    public void SetActivePowerScheme(IPowerScheme newPowerScheme, bool isForce = false)
    {
        var currentPowerScheme = ActivePowerScheme;
        if (newPowerScheme.Guid == currentPowerScheme.Guid && !isForce)
        {
            return;
        }

        PowerManager.SetActivePlan(newPowerScheme.Guid);
        OnActivePowerSchemeChanged(new PowerSchemeEventArgs(currentPowerScheme, newPowerScheme));
    }

    private void SetActivePowerScheme(Guid guid)
        => SetActivePowerScheme((PowerScheme)PowerSchemes.FirstOrDefault(p => p.Guid == guid));

    public void RestoreDefaultPowerSchemes()
        => Watchers.RaiseActionWithoutWatchers(PowerManager.RestoreDefaultPlans);

    private static bool CanCreateExtremePowerScheme
        => RegistryService.ExistsDefaultPowerScheme(SettingSchemes[SettingScheme.Ultimate].Guid);

    public bool ExistsMobilePlatformRole
        => PowerManager.IsMobilePlatformRole();

    public bool ExistsHibernate
        => PowerManager.IsHibernate();

    public bool ExistsSleep
        => PowerManager.IsSleep();

    public void DeleteAllTypicalScheme()
    {
        var activeGuid = ActivePowerScheme.Guid;

        foreach (var guid in SettingSchemes
                     .Where(p => !p.Value.IsNative && RegistryService.ExistsTypicalPowerScheme(p.Value.Guid))
                     .Select(p => p.Value.Guid))
        {
            if (guid == activeGuid)
            {
                SetActivePowerScheme(SettingSchemes[SettingScheme.Balance].Guid);
            }

            PowerManager.DeletePlan(guid);
        }
    }

    public bool ExistsAllTypicalScheme =>
        !SettingSchemes.Where(p => !p.Value.IsNative).Select(s => s.Value.Guid)
            .Except(RegistryService.UserPowerSchemes).Any();

    private void DeleteTypicalScheme(Guid guid)
    {
        if (!ExistsTypicalPowerScheme(guid))
        {
            return;
        }

        var activeGuid = ActivePowerScheme.Guid;

        if (guid == activeGuid)
        {
            SetActivePowerScheme(SettingSchemes[SettingScheme.Balance].Guid);
        }
        PowerManager.DeletePlan(guid);
    }

    public void SetLid(int value)
    {
        var activeScheme = ActivePowerScheme;

        var listGuid = PowerSchemes.Select(p => p.Guid);
        var powerSchemeDcAcValues = new PowerSchemeDcAcValues(value, value);

        foreach (var guid in listGuid)
        {
            var settingLid = new PowerSchemeLid(guid, powerSchemeDcAcValues);
            settingLid.ApplyValues();
        }

        SetActivePowerScheme(activeScheme, true);
    }

    public void CreateTypicalSchemes()
    {
        void CreateTypicalSchemesIn()
        {
            CreateMediaPowerScheme();
            CreateStablePowerScheme();
            CreateSimplePowerScheme();
            CreateExtremePowerScheme();
            SetActivePowerScheme(SettingSchemes[SettingScheme.Stable].Guid);
        }

        Watchers.RaiseActionWithoutWatchers(CreateTypicalSchemesIn);
    }

    private void CreateStablePowerScheme()
    {
        CreateTypicalPowerScheme(SettingSchemes[SettingScheme.High].Guid, SettingSchemes[SettingScheme.Stable].Guid,
            Language.Current.StableName, Language.Current.StableDescription);

        ApplyDefaultValues(SettingScheme.Stable);
    }

    private static void ApplyDefaultValues(SettingScheme settingScheme)
    {
        var settings = new PowerSchemeSettings(settingScheme);
        settings.ApplyDefaultValues();
    }

    private void DeleteStablePowerScheme()
        => DeleteTypicalScheme(SettingSchemes[SettingScheme.Stable].Guid);

    private void CreateMediaPowerScheme()
    {
        CreateTypicalPowerScheme(SettingSchemes[SettingScheme.Balance].Guid,
            SettingSchemes[SettingScheme.Media].Guid,
            Language.Current.MediaName, Language.Current.MediaDescription);

        ApplyDefaultValues(SettingScheme.Media);
    }

    private void DeleteMediaPowerScheme()
        => DeleteTypicalScheme(SettingSchemes[SettingScheme.Media].Guid);

    private void CreateSimplePowerScheme()
    {
        CreateTypicalPowerScheme(SettingSchemes[SettingScheme.Low].Guid, SettingSchemes[SettingScheme.Simple].Guid,
            Language.Current.SimpleName, Language.Current.SimpleDescription);

        ApplyDefaultValues(SettingScheme.Simple);
    }

    private void DeleteSimplePowerScheme()
        => DeleteTypicalScheme(SettingSchemes[SettingScheme.Simple].Guid);

    private void CreateExtremePowerScheme()
    {
        if (CanCreateExtremePowerScheme)
        {
            CreateTypicalPowerScheme(SettingSchemes[SettingScheme.Ultimate].Guid, SettingSchemes[SettingScheme.Extreme].Guid,
                Language.Current.ExtremeName, Language.Current.ExtremeDescription);
        }

        ApplyDefaultValues(SettingScheme.Extreme);
    }

    private static bool ExistsStablePowerScheme
        => ExistsTypicalPowerScheme(SettingSchemes[SettingScheme.Stable].Guid);

    private static bool ExistsMediaPowerScheme
        => ExistsTypicalPowerScheme(SettingSchemes[SettingScheme.Media].Guid);

    private static bool ExistsSimplePowerScheme
        => ExistsTypicalPowerScheme(SettingSchemes[SettingScheme.Simple].Guid);

    private static bool ExistsExtremePowerScheme
        => ExistsTypicalPowerScheme(SettingSchemes[SettingScheme.Extreme].Guid);

    private void DeleteExtremePowerScheme()
        => DeleteTypicalScheme(SettingSchemes[SettingScheme.Extreme].Guid);

    private static StatePowerScheme ToggledStatePowerScheme(StatePowerScheme statePowerScheme, bool b)
    {
        return b ? new StatePowerScheme(statePowerScheme.PowerScheme, ActionWithPowerScheme.Delete)
            : new StatePowerScheme(statePowerScheme.PowerScheme, ActionWithPowerScheme.Create);

    }

    public StatePowerScheme? StatePowerSchemeToggle(StatePowerScheme statePowerScheme)
    {
        var guid = statePowerScheme.PowerScheme.Guid;

        if (guid == SettingSchemes[SettingScheme.Stable].Guid)
        {
            return ToggledStatePowerScheme(statePowerScheme, ExistsStablePowerScheme);
        }

        if (guid == SettingSchemes[SettingScheme.Media].Guid)
        {
            return ToggledStatePowerScheme(statePowerScheme, ExistsMediaPowerScheme);
        }

        if (guid == SettingSchemes[SettingScheme.Simple].Guid)
        {
            return ToggledStatePowerScheme(statePowerScheme, ExistsSimplePowerScheme);
        }

        if (guid == SettingSchemes[SettingScheme.Extreme].Guid)
        {
            return ToggledStatePowerScheme(statePowerScheme, ExistsExtremePowerScheme);
        }

        return null;
    }

    private static void ApplyAction(
        ActionWithPowerScheme actionWithPowerScheme,
        Action create,
        Action delete)
    {
        switch (actionWithPowerScheme)
        {
            case ActionWithPowerScheme.Create:
                create();
                break;
            case ActionWithPowerScheme.Delete:
                delete();
                break;
            default:
                throw new AccessViolationException(nameof(ActionWithPowerScheme));
        }
    }

    public void ActionPowerScheme(StatePowerScheme statePowerScheme)
    {
        var guid = statePowerScheme.PowerScheme.Guid;

        if (statePowerScheme.Value is not ActionWithPowerScheme value)
        {
            throw new ArgumentOutOfRangeException(nameof(statePowerScheme));
        }

        if (guid == SettingSchemes[SettingScheme.Stable].Guid)
        {
            ApplyAction(value, CreateStablePowerScheme, DeleteStablePowerScheme);
        }

        if (guid == SettingSchemes[SettingScheme.Media].Guid)
        {
            ApplyAction(value, CreateMediaPowerScheme, DeleteMediaPowerScheme);
        }

        if (guid == SettingSchemes[SettingScheme.Simple].Guid)
        {
            ApplyAction(value, CreateSimplePowerScheme, DeleteSimplePowerScheme);
        }

        if (guid == SettingSchemes[SettingScheme.Extreme].Guid)
        {
            ApplyAction(value, CreateExtremePowerScheme, DeleteExtremePowerScheme);
        }
    }

    public string TextActionToggle(StatePowerScheme statePowerScheme)
    {
        var guid = statePowerScheme.PowerScheme.Guid;

        if (guid == SettingSchemes[SettingScheme.Stable].Guid)
        {
            return ExistsStablePowerScheme
                ? Language.Current.DeleteStableScheme
                : Language.Current.CreateStableScheme;
        }

        if (guid == SettingSchemes[SettingScheme.Media].Guid)
        {
            return ExistsMediaPowerScheme
                ? Language.Current.DeleteMediaScheme
                : Language.Current.CreateMediaScheme;
        }

        if (guid == SettingSchemes[SettingScheme.Simple].Guid)
        {
            return ExistsSimplePowerScheme
                ? Language.Current.DeleteSimpleScheme
                : Language.Current.CreateSimpleScheme;
        }

        if (guid == SettingSchemes[SettingScheme.Extreme].Guid)
        {
            return ExistsExtremePowerScheme
                ? Language.Current.DeleteExtremeScheme
                : Language.Current.CreateExtremeScheme;
        }

        return string.Empty;
    }

    private static void CreateTypicalPowerScheme(
        Guid source,
        Guid destination,
        string name,
        string? description = null)
    {
        var isExistsTypicalPowerScheme = ExistsTypicalPowerScheme(destination);
        if (isExistsTypicalPowerScheme)
        {
            return;
        }

        PowerManager.DuplicatePlan(source, destination);

        PowerManager.SetPlanName(destination, name);

        if (description == null)
        {
            return;
        }

        PowerManager.SetPlanDescription(destination, description);
    }

    private static bool ExistsTypicalPowerScheme(Guid guid)
        => RegistryService.ExistsTypicalPowerScheme(guid);

    public Watchers Watchers { get; } = new();

    public event EventHandler<PowerSchemeEventArgs>? ActivePowerSchemeChanged;

    private void OnActivePowerSchemeChanged(PowerSchemeEventArgs e)
    {
        ActivePowerSchemeChanged?.Invoke(this, e);
    }

    private static IPowerScheme NewPowerScheme(Guid guid)
    {
        var powerSchemeParams =
            SettingSchemes
                .Where(p => p.Value.Guid == guid)
                .Select(p => p.Value).FirstOrDefault();

        return powerSchemeParams
               ?? new PowerScheme(guid, false, ImageItem.Unknown);
    }
}
