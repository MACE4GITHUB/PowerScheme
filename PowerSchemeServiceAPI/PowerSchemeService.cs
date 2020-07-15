namespace PowerSchemeServiceAPI
{
    using Common;
    using EventsArgs;
    using Languages;
    using Model;
    using PowerManagerAPI;
    using RegistryManager;
    using Settings;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static PowerSchemeLookup;

    public class PowerSchemeService : IPowerSchemeService
    {
        public IEnumerable<IPowerScheme> DefaultPowerSchemes
            => PowerSchemeItems
                .Where(p => p.Value.IsNative && p.Value.IsVisible)
                .Select(p => NewPowerScheme(p.Value));

        public IEnumerable<IPowerScheme> TypicalPowerSchemes
        {
            get
            {
                var typicalGuid
                    = PowerSchemeItems
                        .Where(p => !p.Value.IsNative && p.Value.IsVisible);

                return CanCreateExtremePowerScheme
                    ? typicalGuid.Select(p => NewPowerScheme(p.Value))
                    : typicalGuid.SkipWhile(p => p.Key == SchemeItem.Ultimate)
                    .Select(p => NewPowerScheme(p.Value));
            }
        }

        public IEnumerable<IPowerScheme> UserPowerSchemes
            => RegistryService.UserPowerSchemes.Select(NewPowerScheme);

        private IEnumerable<IPowerScheme> PowerSchemes
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

        public void SetActivePowerScheme(IPowerScheme powerScheme, bool isForce = false)
        {
            if (powerScheme.Guid == ActivePowerScheme.Guid && !isForce) return;

            PowerManager.SetActivePlan(powerScheme.Guid);
            OnActivePowerSchemeChanged(new PowerSchemeEventArgs(powerScheme));
        }

        private void SetActivePowerScheme(Guid guid)
            => SetActivePowerScheme((PowerScheme)PowerSchemes.FirstOrDefault(p => p.Guid == guid));

        public void RestoreDefaultPowerSchemes()
            => Watchers.RaiseActionWithoutWatchers(PowerManager.RestoreDefaultPlans);

        private bool CanCreateExtremePowerScheme
            => RegistryService.ExistsDefaultPowerScheme(PowerSchemeItems[SchemeItem.Ultimate].Guid);

        public bool ExistsMobilePlatformRole
            => PowerManager.IsMobilePlatformRole();

        public bool ExistsHibernate
            => PowerManager.IsHibernate();

        public bool ExistsSleep
            => PowerManager.IsSleep();

        public void DeleteAllTypicalScheme()
        {
            var activeGuid = ActivePowerScheme.Guid;

            foreach (var guid in PowerSchemeItems
                .Where(p => !p.Value.IsNative && RegistryService.ExistsTypicalPowerScheme(p.Value.Guid))
                .Select(p => p.Value.Guid))
            {
                if (guid == activeGuid)
                {
                    SetActivePowerScheme(PowerSchemeItems[SchemeItem.Balance].Guid);
                }
                PowerManager.DeletePlan(guid);
            }
        }

        public bool ExistsAllTypicalScheme =>
            !PowerSchemeItems.Where(p => !p.Value.IsNative).Select(s => s.Value.Guid.ToString())
                .Except(RegistryService.UserPowerSchemes).Any();

        private void DeleteTypicalScheme(Guid guid)
        {
            if (!ExistsTypicalPowerScheme(guid)) return;

            var activeGuid = ActivePowerScheme.Guid;

            if (guid == activeGuid)
            {
                SetActivePowerScheme(PowerSchemeItems[SchemeItem.Balance].Guid);
            }
            PowerManager.DeletePlan(guid);
        }

        public void SetLid(int value)
        {
            var activeScheme = ActivePowerScheme;

            var listGuid = AllPowerSchemes.Select(p => p.Guid);
            var powerSchemeDCACValues = new PowerSchemeDCACValues(value, value);

            foreach (var guid in listGuid)
            {
                var settingLid = new PowerSchemeLid(guid, powerSchemeDCACValues);
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
                SetActivePowerScheme(PowerSchemeItems[SchemeItem.Stable].Guid);
            }

            Watchers.RaiseActionWithoutWatchers(CreateTypicalSchemesIn);
        }

        private void CreateStablePowerScheme()
        {
            CreateTypicalPowerScheme(PowerSchemeItems[SchemeItem.High].Guid, PowerSchemeItems[SchemeItem.Stable].Guid,
                Language.Current.StableName, Language.Current.StableDescription);

            var settings = new PowerSchemeSettings(PowerSchemeItems[SchemeItem.Stable].Guid);

            settings.ApplyDefaultValues();
        }

        private void DeleteStablePowerScheme()
            => DeleteTypicalScheme(PowerSchemeItems[SchemeItem.Stable].Guid);

        private void CreateMediaPowerScheme() =>
            CreateTypicalPowerScheme(PowerSchemeItems[SchemeItem.Balance].Guid, PowerSchemeItems[SchemeItem.Media].Guid,
                Language.Current.MediaName, Language.Current.MediaDescription);

        private void DeleteMediaPowerScheme()
            => DeleteTypicalScheme(PowerSchemeItems[SchemeItem.Media].Guid);

        private void CreateSimplePowerScheme() =>
            CreateTypicalPowerScheme(PowerSchemeItems[SchemeItem.Low].Guid, PowerSchemeItems[SchemeItem.Simple].Guid,
                Language.Current.SimpleName, Language.Current.SimpleDescription);

        private void DeleteSimplePowerScheme()
            => DeleteTypicalScheme(PowerSchemeItems[SchemeItem.Simple].Guid);

        private void CreateExtremePowerScheme()
        {
            if (CanCreateExtremePowerScheme)
                CreateTypicalPowerScheme(PowerSchemeItems[SchemeItem.Ultimate].Guid, PowerSchemeItems[SchemeItem.Extreme].Guid,
                    Language.Current.ExtremeName, Language.Current.ExtremeDescription);
        }

        private bool ExistsStablePowerScheme
            => ExistsTypicalPowerScheme(PowerSchemeItems[SchemeItem.Stable].Guid);

        private bool ExistsMediaPowerScheme
            => ExistsTypicalPowerScheme(PowerSchemeItems[SchemeItem.Media].Guid);

        private bool ExistsSimplePowerScheme
            => ExistsTypicalPowerScheme(PowerSchemeItems[SchemeItem.Simple].Guid);

        private bool ExistsExtremePowerScheme
            => ExistsTypicalPowerScheme(PowerSchemeItems[SchemeItem.Extreme].Guid);

        private void DeleteExtremePowerScheme()
            => DeleteTypicalScheme(PowerSchemeItems[SchemeItem.Extreme].Guid);

        private StatePowerScheme ToggledStatePowerScheme(StatePowerScheme statePowerScheme, bool b)
        {
            return b ? new StatePowerScheme(statePowerScheme.PowerScheme, ActionWithPowerScheme.Delete)
                     : new StatePowerScheme(statePowerScheme.PowerScheme, ActionWithPowerScheme.Create);

        }

        public StatePowerScheme StatePowerSchemeToggle(StatePowerScheme statePowerScheme)
        {
            var guid = statePowerScheme.PowerScheme.Guid;

            if (guid == PowerSchemeItems[SchemeItem.Stable].Guid)
            {
                return ToggledStatePowerScheme(statePowerScheme, ExistsStablePowerScheme);
            }

            if (guid == PowerSchemeItems[SchemeItem.Media].Guid)
            {
                return ToggledStatePowerScheme(statePowerScheme, ExistsMediaPowerScheme);
            }

            if (guid == PowerSchemeItems[SchemeItem.Simple].Guid)
            {
                return ToggledStatePowerScheme(statePowerScheme, ExistsSimplePowerScheme);
            }

            if (guid == PowerSchemeItems[SchemeItem.Extreme].Guid)
            {
                return ToggledStatePowerScheme(statePowerScheme, ExistsExtremePowerScheme);
            }

            return null;
        }

        private void ApplyAction(ActionWithPowerScheme actionWithPowerScheme,
            Action create, Action delete)
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
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void ActionPowerScheme(StatePowerScheme statePowerScheme)
        {
            var guid = statePowerScheme.PowerScheme.Guid;
            var value = (ActionWithPowerScheme)statePowerScheme.Value;

            if (guid == PowerSchemeItems[SchemeItem.Stable].Guid)
            {
                ApplyAction(value, CreateStablePowerScheme, DeleteStablePowerScheme);
            }

            if (guid == PowerSchemeItems[SchemeItem.Media].Guid)
            {
                ApplyAction(value, CreateMediaPowerScheme, DeleteMediaPowerScheme);
            }

            if (guid == PowerSchemeItems[SchemeItem.Simple].Guid)
            {
                ApplyAction(value, CreateSimplePowerScheme, DeleteSimplePowerScheme);
            }

            if (guid == PowerSchemeItems[SchemeItem.Extreme].Guid)
            {
                ApplyAction(value, CreateExtremePowerScheme, DeleteExtremePowerScheme);
            }
        }

        public string TextActionToggle(StatePowerScheme statePowerScheme)
        {
            var guid = statePowerScheme.PowerScheme.Guid;

            if (guid == PowerSchemeItems[SchemeItem.Stable].Guid)
            {
                return ExistsStablePowerScheme
                    ? Language.Current.DeleteStableScheme
                    : Language.Current.CreateStableScheme;
            }

            if (guid == PowerSchemeItems[SchemeItem.Media].Guid)
            {
                return ExistsMediaPowerScheme
                    ? Language.Current.DeleteMediaScheme
                    : Language.Current.CreateMediaScheme;
            }

            if (guid == PowerSchemeItems[SchemeItem.Simple].Guid)
            {
                return ExistsSimplePowerScheme
                    ? Language.Current.DeleteSimpleScheme
                    : Language.Current.CreateSimpleScheme;
            }

            if (guid == PowerSchemeItems[SchemeItem.Extreme].Guid)
            {
                return ExistsExtremePowerScheme
                    ? Language.Current.DeleteExtremeScheme
                    : Language.Current.CreateExtremeScheme;
            }

            return null;
        }

        private IEnumerable<IPowerScheme> AllPowerSchemes
            => DefaultPowerSchemes.Concat(UserPowerSchemes);

        private void CreateTypicalPowerScheme(Guid source, Guid destination, string name, string description = null)
        {
            var isExistsTypicalPowerScheme = ExistsTypicalPowerScheme(destination);
            if (isExistsTypicalPowerScheme) return;
            PowerManager.DuplicatePlan(source, destination);

            PowerManager.SetPlanName(destination, name);

            if (description == null) return;
            PowerManager.SetPlanDescription(destination, description);
        }

        private static bool ExistsTypicalPowerScheme(Guid guid)
            => RegistryService.ExistsTypicalPowerScheme(guid);

        public Watchers Watchers { get; } = new Watchers();

        public event EventHandler<PowerSchemeEventArgs> ActivePowerSchemeChanged;

        private void OnActivePowerSchemeChanged(PowerSchemeEventArgs e)
        {
            ActivePowerSchemeChanged?.Invoke(this, e);
        }

        private static PowerScheme NewPowerScheme(PowerSchemeParams powerSchemeParams)
        {
            return new PowerScheme(
                powerSchemeParams.Guid,
                powerSchemeParams.IsNative,
                powerSchemeParams.Image,
                powerSchemeParams.IsVisible);
        }
        private static PowerScheme NewPowerScheme(string guid)
        {
            var powerSchemeParams =
                PowerSchemeItems
                    .Where(p => p.Value.Guid.ToString() == guid)
                    .Select(p => p.Value).FirstOrDefault();

            return powerSchemeParams != null
                ? NewPowerScheme(powerSchemeParams)
                : new PowerScheme(Guid.Parse(guid), false, ImageItem.Unknown, true);
        }
    }
}
