namespace PowerScheme.Services
{
    using Languages;
    using PowerManagerAPI;
    using PowerSchemeServiceAPI;
    using PowerSchemeServiceAPI.EventsArgs;
    using PowerSchemeServiceAPI.Model;
    using PowerSchemeServiceAPI.Settings;
    using RegistryManager;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PowerSchemeService : IPowerSchemeService
    {
        private static readonly Guid HIGH_SCHEME_GUID = new Guid("8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c");
        private static readonly Guid BALANCE_SCHEME_GUID = new Guid("381b4222-f694-41f0-9685-ff5bb260df2e");
        private static readonly Guid LOW_SCHEME_GUID = new Guid("a1841308-3541-4fab-bc81-f71556f20b4a");
        private static readonly Guid ULTIMATE_SCHEME_GUID = new Guid("e9a42b02-d5df-448d-aa00-03f14749eb61");

        private static readonly Guid STABLE_SCHEME_GUID = new Guid("fa0cd8f1-1300-4710-820b-00e8e75f31f8");
        private static readonly Guid MEDIA_SCHEME_GUID = new Guid("fcab38a3-7e4c-4a75-8483-f522befb9c58");
        private static readonly Guid SIMPLE_SCHEME_GUID = new Guid("fa8d915c-65de-4bba-9569-3c2e77ea68b6");
        private static readonly Guid EXTREME_SCHEME_GUID = new Guid("f384acfa-ed71-4607-bf8e-747d56402f0c");

        private const string UNKNOWN_ICON = "Unknown";

        private static readonly Dictionary<Guid, string> NATIVES_GUID = new Dictionary<Guid, string>
        {
            {HIGH_SCHEME_GUID, "High"},
            {BALANCE_SCHEME_GUID, "Balance"},
            {LOW_SCHEME_GUID, "Low"}
        };

        private static readonly Dictionary<Guid, string> TYPICAL_GUID = new Dictionary<Guid, string>
        {
            {STABLE_SCHEME_GUID, "Stable"},
            {MEDIA_SCHEME_GUID, "Media"},
            {SIMPLE_SCHEME_GUID, "Simple"},
            {EXTREME_SCHEME_GUID, "Extreme"}
        };

        public PowerSchemeService()
        {

        }

        public IEnumerable<IPowerScheme> DefaultPowerSchemes
            => NATIVES_GUID.Keys.Select(NewPowerScheme);

        public IEnumerable<IPowerScheme> UserPowerSchemes
            => RegistryService.UserPowerSchemes.Select(g => NewPowerScheme(Guid.Parse(g)));

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

        /// <summary>
        /// Set the Active Power Scheme.
        /// </summary>
        /// <param name="powerScheme"></param>
        /// <param name="isForce">Need to apply new AC & DC values</param>
        public void SetActivePowerScheme(IPowerScheme powerScheme, bool isForce = false)
        {
            if (powerScheme.Guid == ActivePowerScheme.Guid && !isForce) return;

            PowerManager.SetActivePlan(powerScheme.Guid);
            OnActivePowerSchemeChanged(new PowerSchemeEventArgs(powerScheme));
        }

        public void SetActivePowerScheme(Guid guid)
            => SetActivePowerScheme((PowerSchemeServiceAPI.Model.PowerScheme)PowerSchemes.FirstOrDefault(p => p.Guid == guid));

        public void RestoreDefaultPowerSchemes()
            => Watchers.RaiseActionWithoutWatchers(PowerManager.RestoreDefaultPlans);

        public bool CanCreateExtremePowerScheme
            => RegistryService.ExistsDefaultPowerScheme(ULTIMATE_SCHEME_GUID);

        public bool ExistsMobilePlatformRole
            => PowerManager.IsMobilePlatformRole();

        public bool ExistsHibernate
            => PowerManager.IsHibernate();

        public bool ExistsSleep
            => PowerManager.IsSleep();

        public void DeleteAllTypicalScheme()
        {
            var activeGuid = ActivePowerScheme.Guid;
            foreach (var guid in TYPICAL_GUID.Keys.Where(RegistryService.ExistsTypicalPowerScheme))
            {
                if (guid == activeGuid)
                {
                    SetActivePowerScheme(BALANCE_SCHEME_GUID);
                }
                PowerManager.DeletePlan(guid);
            }
        }

        public bool ExistsAllTypicalScheme =>
            !TYPICAL_GUID.Keys.Select(s => s.ToString())
                .Except(RegistryService.UserPowerSchemes).Any();

        public void DeleteTypicalScheme(Guid guid)
        {
            if (!ExistsTypicalPowerScheme(guid)) return;

            var activeGuid = ActivePowerScheme.Guid;

            if (guid == activeGuid)
            {
                SetActivePowerScheme(BALANCE_SCHEME_GUID);
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
                SetActivePowerScheme(STABLE_SCHEME_GUID);
            }

            Watchers.RaiseActionWithoutWatchers(CreateTypicalSchemesIn);
        }

        public void CreateStablePowerScheme()
        {
            CreateTypicalPowerScheme(HIGH_SCHEME_GUID, STABLE_SCHEME_GUID,
                Language.Current.StableName, Language.Current.StableDescription);

            var settings = new PowerSchemeSettings(STABLE_SCHEME_GUID);

            settings.ApplyDefaultValues();
        }

        public void DeleteStablePowerScheme()
            => DeleteTypicalScheme(STABLE_SCHEME_GUID);

        public void CreateMediaPowerScheme() =>
            CreateTypicalPowerScheme(BALANCE_SCHEME_GUID, MEDIA_SCHEME_GUID,
                Language.Current.MediaName, Language.Current.MediaDescription);

        public void DeleteMediaPowerScheme()
            => DeleteTypicalScheme(MEDIA_SCHEME_GUID);

        public void CreateSimplePowerScheme() =>
            CreateTypicalPowerScheme(LOW_SCHEME_GUID, SIMPLE_SCHEME_GUID,
                Language.Current.SimpleName, Language.Current.SimpleDescription);

        public void DeleteSimplePowerScheme()
            => DeleteTypicalScheme(SIMPLE_SCHEME_GUID);

        public void CreateExtremePowerScheme()
        {
            if (CanCreateExtremePowerScheme)
                CreateTypicalPowerScheme(ULTIMATE_SCHEME_GUID, EXTREME_SCHEME_GUID,
                    Language.Current.ExtremeName, Language.Current.ExtremeDescription);
        }

        public bool ExistsStablePowerScheme
            => ExistsTypicalPowerScheme(STABLE_SCHEME_GUID);

        public bool ExistsMediaPowerScheme
            => ExistsTypicalPowerScheme(MEDIA_SCHEME_GUID);

        public bool ExistsSimplePowerScheme
            => ExistsTypicalPowerScheme(SIMPLE_SCHEME_GUID);

        public bool ExistsExtremePowerScheme
            => ExistsTypicalPowerScheme(EXTREME_SCHEME_GUID);

        public void DeleteExtremePowerScheme()
            => DeleteTypicalScheme(EXTREME_SCHEME_GUID);

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

        private bool ContainsNative(Guid guid) =>
            NATIVES_GUID.Keys.Contains(guid);

        private bool ContainsTypical(Guid guid) =>
            TYPICAL_GUID.Keys.Contains(guid);

        private PowerSchemeServiceAPI.Model.PowerScheme NewPowerScheme(Guid guid)
        {
            var isNative = ContainsNative(guid);
            var isTypical = ContainsTypical(guid);

            string image;

            if (isNative)
            {
                image = NATIVES_GUID[guid];
            }
            else if (isTypical)
            {
                image = TYPICAL_GUID[guid];
            }
            else
            {
                image = UNKNOWN_ICON;
            }

            return new PowerSchemeServiceAPI.Model.PowerScheme(guid, isNative, image);
        }
    }
}
