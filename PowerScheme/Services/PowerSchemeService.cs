using PowerManagerAPI;
using PowerScheme.EventsArgs;
using PowerScheme.Model;
using PowerScheme.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerScheme.Services
{
    public class PowerSchemeService : IPowerSchemeService
    {
        private static readonly Guid HIGH_SCHEME_GUID = new Guid("8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c");
        private static readonly Guid BALANCE_SCHEME_GUID = new Guid("381b4222-f694-41f0-9685-ff5bb260df2e");
        private static readonly Guid LOW_SCHEME_GUID = new Guid("a1841308-3541-4fab-bc81-f71556f20b4a");

        private static readonly Guid STABLE_SCHEME_GUID = new Guid("fa0cd8f1-1300-4710-820b-00e8e75f31f8");
        private static readonly Guid MEDIA_SCHEME_GUID = new Guid("fcab38a3-7e4c-4a75-8483-f522befb9c58");
        private static readonly Guid SIMPLE_SCHEME_GUID = new Guid("fa8d915c-65de-4bba-9569-3c2e77ea68b6");

        private static readonly Dictionary<Guid, string> _nativesGuid = new Dictionary<Guid, string>
        {
            {HIGH_SCHEME_GUID, "High"},
            {BALANCE_SCHEME_GUID, "Balance"},
            {LOW_SCHEME_GUID, "Low"}
        };

        private static readonly Dictionary<Guid, string> _typicalGuid = new Dictionary<Guid, string>
        {
            {STABLE_SCHEME_GUID, "Stable"},
            {MEDIA_SCHEME_GUID, "Media"},
            {SIMPLE_SCHEME_GUID, "Simple"}
        };

        public PowerSchemeService()
        {

        }

        public IEnumerable<IPowerScheme> DefaultPowerSchemes
            => _nativesGuid.Keys.Select(NewPowerScheme);

        public IEnumerable<IPowerScheme> UserPowerSchemes
            => RegistryService.UserPowerSchemes.Select(g => NewPowerScheme(Guid.Parse(g)));

        public IPowerScheme FirstAnyPowerScheme
            => AllPowerSchemes.FirstOrDefault();

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

        public bool IsNeedAdminAccessForChangePowerScheme()
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
            => SetActivePowerScheme((Model.PowerScheme)PowerSchemes.FirstOrDefault(p => p.Guid == guid));

        public void RestoreDefaultPowerSchemes()
            => Watchers.RaiseActionWithoutWatchers(PowerManager.RestoreDefaultPlans);

        public bool IsMobilePlatformRole()
            => PowerManager.IsMobilePlatformRole();

        public bool IsHibernate()
            => PowerManager.IsHibernate();

        public bool IsSleep()
            => PowerManager.IsSleep();

        public void DeleteTypicalScheme()
        {
            var activeGuid = ActivePowerScheme.Guid;
            foreach (var guid in _typicalGuid.Keys.Where(RegistryService.IsExistsTypicalPowerScheme))
            {
                if (guid == activeGuid)
                {
                    SetActivePowerScheme(BALANCE_SCHEME_GUID);
                }
                PowerManager.DeletePlan(guid);
            }
        }

        public void CreateStablePowerScheme(string name, string description = null)
        {
            CreateTypicalPowerScheme(HIGH_SCHEME_GUID, STABLE_SCHEME_GUID,
                name, description);

            var settings = new PowerSchemeSettings(STABLE_SCHEME_GUID);

            settings.ApplyDefaultValues();
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

        public void CreateMediaPowerScheme(string name, string description = null) =>
            CreateTypicalPowerScheme(BALANCE_SCHEME_GUID, MEDIA_SCHEME_GUID,
                name, description);

        public void CreateSimplePowerScheme(string name, string description = null) =>
            CreateTypicalPowerScheme(LOW_SCHEME_GUID, SIMPLE_SCHEME_GUID,
                name, description);

        private IEnumerable<IPowerScheme> AllPowerSchemes
            => DefaultPowerSchemes.Concat(UserPowerSchemes);

        private void CreateTypicalPowerScheme(Guid source, Guid destination, string name, string description = null)
        {
            var isExistsTypicalPowerScheme = IsExistsTypicalPowerScheme(destination);
            if (isExistsTypicalPowerScheme) return;
            PowerManager.DuplicatePlan(source, destination);

            PowerManager.SetPlanName(destination, name);

            if (description == null) return;
            PowerManager.SetPlanDescription(destination, description);
        }

        private static bool IsExistsTypicalPowerScheme(Guid guid)
            => RegistryService.IsExistsTypicalPowerScheme(guid);

        public Watchers Watchers { get; } = new Watchers();

        public event EventHandler<PowerSchemeEventArgs> ActivePowerSchemeChanged;

        private void OnActivePowerSchemeChanged(PowerSchemeEventArgs e)
        {
            ActivePowerSchemeChanged?.Invoke(this, e);
        }

        private bool ContainsNative(Guid guid) =>
            _nativesGuid.Keys.Contains(guid);

        private bool ContainsTypical(Guid guid) =>
            _typicalGuid.Keys.Contains(guid);

        private Model.PowerScheme NewPowerScheme(Guid guid)
        {
            var isNative = ContainsNative(guid);
            var isTypical = ContainsTypical(guid);

            string image;

            if (isNative)
            {
                image = _nativesGuid[guid];
            }
            else if (isTypical)
            {
                image = _typicalGuid[guid];
            }
            else
            {
                image = null;
            }

            return new Model.PowerScheme(guid, isNative, image);
        }
    }
}
