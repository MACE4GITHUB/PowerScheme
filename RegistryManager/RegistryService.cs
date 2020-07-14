using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using Common;
using Microsoft.Win32;
using PowerManagerAPI;
using RegistryManager.Model;
using static RegistryManager.Registry;

namespace RegistryManager
{
    public class RegistryService
    {
        public static string GetActiveScheme()
        {
            return GetSettings<string>(RegActivePowerScheme);
        }

        public static bool IsRunOnStartup =>
             GetSettings<string>(RegRunOnStartup) != null;

        public static bool IsShowHibernateOption =>
            GetSettings<int>(RegShowHibernateOption) == 1;

        public static bool IsShowSleepOption =>
            GetSettings<int>(RegShowSleepOption) == 1;

        public static bool IsShowLockOption =>
            GetSettings<int>(RegShowLockOption) == 1;


        public static void SetStartup(bool isStart)
        {
            var registryParam = RegStartup;

            if (isStart) SaveSetting(registryParam);
            else DeleteSetting(registryParam);
        }

        public static string GetFriendlyNamePowerScheme(Guid guid)
        {
            return GetSettings<string>(RegFriendlyNamePowerSchemes(guid));
        }

        public static string GetDescriptionPowerScheme(Guid guid)
        {
            return GetSettings<string>(RegDescriptionPowerSchemes(guid));
        }

        public static int GetLidOption(Guid guid)
        {
            var value = GetSettings<int>(RegLidOption(guid));
            return !ExistsSettings(RegLidOption(guid)) ? 1 : value;
        }

        public static bool IsFirstStart(string company, string product)
        {
            return !ExistsSettings(RegAppSettings(company, product));
        }

        public static bool ExistsTypicalPowerScheme(Guid guid)
            => GetSettings<string>(RegPowerSchemes(guid)) != null;

        public static bool ExistsDefaultPowerScheme(Guid guid)
            => GetSettings<string>(RegPowerSchemes(guid,true)) != null;

        private static IEnumerable<string> DefaultPowerSchemes => GetSubKeys(RegDefaultPowerSchemes);

        private static IEnumerable<string> CurrentPowerSchemes => GetSubKeys(RegCurrentPowerSchemes);

        public static IEnumerable<string> UserPowerSchemes => CurrentPowerSchemes.Except(DefaultPowerSchemes).ToArray();

        public static RegistryWatcher<string> ActivePowerSchemeRegistryWatcher()
            => new RegistryWatcher<string>(RegActivePowerScheme)
            {
                RegChangeNotifyFilter = RegChangeNotifyFilter.Value
            };

        public static RegistryWatcher<string> PowerSchemesRegistryWatcher()
            => new RegistryWatcher<string>(RegCurrentPowerSchemes)
            {
                RegChangeNotifyFilter = RegChangeNotifyFilter.Key
            };

        public static void SetHibernateOption(ResourceManager resourceManager, object value)
        {
            SetRegistryValue(resourceManager, RegShowHibernateOption, value);
        }

        public static void SetSleepOption(ResourceManager resourceManager, object value)
        {
            SetRegistryValue(resourceManager, RegShowSleepOption, value);
        }

        public static void SetAppSettings(string company, string product, string name, object value)
        {
            var regAppSettings = RegAppSettings(company, product);
            regAppSettings.Name = name;
            regAppSettings.Value = value;
            SaveSetting(regAppSettings);
        }

        private static void SetRegistryValue(ResourceManager resourceManager, RegistryParam registryParam, object value)
        {
            registryParam.Value = value;
            SaveRegistryEx(resourceManager, registryParam);
        }

        private static void SaveRegistryEx(ResourceManager resourceManager, RegistryParam registryParam)
        {
            var executorRegistryService =
                new ExecutorRegistryService(
                    resourceManager,
                    registryParam,
                    RegistryAdminAction.set,
                    Guid.NewGuid());

            executorRegistryService.Execute();
        }

        private static RegistryParam RegActivePowerScheme =>
            new RegistryParam()
            {
                RegistryHive = RegistryHive.LocalMachine,
                Path = @"SYSTEM\CurrentControlSet\Control\Power\User",
                Section = "PowerSchemes",
                Name = "ActivePowerScheme",
                Value = PowerManager.GetActivePlan().ToString()
            };

        private static RegistryParam RegDefaultPowerSchemes =>
            new RegistryParam()
            {
                RegistryHive = RegistryHive.LocalMachine,
                Path = @"SYSTEM\CurrentControlSet\Control\Power\User\Default",
                Section = "PowerSchemes"
            };

        private static RegistryParam RegCurrentPowerSchemes =>
            new RegistryParam()
            {
                RegistryHive = RegistryHive.LocalMachine,
                Path = @"SYSTEM\CurrentControlSet\Control\Power\User",
                Section = "PowerSchemes"
            };

        private static RegistryParam RegFriendlyNamePowerSchemes(Guid guid) =>
            new RegistryParam()
            {
                RegistryHive = RegistryHive.LocalMachine,
                Path = @"SYSTEM\CurrentControlSet\Control\Power\User\PowerSchemes",
                Section = guid.ToString(),
                Name = "FriendlyName",
            };

        private static RegistryParam RegDescriptionPowerSchemes(Guid guid) =>
            new RegistryParam()
            {
                RegistryHive = RegistryHive.LocalMachine,
                Path = @"SYSTEM\CurrentControlSet\Control\Power\User\PowerSchemes",
                Section = guid.ToString(),
                Name = "Description",
            };

        private static RegistryParam RegPowerSchemes(Guid guid, bool isDefault = false) =>
            new RegistryParam()
            {
                RegistryHive = RegistryHive.LocalMachine,
                Path = @"SYSTEM\CurrentControlSet\Control\Power\User" +
                       (isDefault ? @"\Default" : "") +
                       @"\PowerSchemes",
                Section = guid.ToString()
            };

        private static RegistryParam RegRunOnStartup =>
            new RegistryParam()
            {
                RegistryHive = RegistryHive.CurrentUser,
                Path = @"SOFTWARE\Microsoft\Windows\CurrentVersion",
                Section = "Run",
                Name = "PowerScheme",
            };

        private static RegistryParam RegAppSettings(string company, string product) =>
            new RegistryParam()
            {
                RegistryHive = RegistryHive.CurrentUser,
                Path = @"SOFTWARE\" + company,
                Section = product
            };

        private static RegistryParam RegStartup
        {
            get
            {
                var rp = RegRunOnStartup;
                rp.Value = Paths.ApplicationFullName;
                return rp;
            }
        }

        private static RegistryParam RegShowHibernateOption =>
            new RegistryParam()
            {
                RegistryHive = RegistryHive.LocalMachine,
                Path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer",
                Section = "FlyoutMenuSettings",
                Name = "ShowHibernateOption",
            };

        private static RegistryParam RegShowSleepOption =>
            new RegistryParam()
            {
                RegistryHive = RegistryHive.LocalMachine,
                Path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer",
                Section = "FlyoutMenuSettings",
                Name = "ShowSleepOption",
            };

        private static RegistryParam RegShowLockOption =>
            new RegistryParam()
            {
                RegistryHive = RegistryHive.LocalMachine,
                Path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer",
                Section = "FlyoutMenuSettings",
                Name = "ShowLockOption",
            };

        private static RegistryParam RegLidOption(Guid guid, string settingIndex = "ACSettingIndex") =>
            new RegistryParam()
            {
                RegistryHive = RegistryHive.LocalMachine,
                Path = @"SYSTEM\CurrentControlSet\Control\Power\User\PowerSchemes\"
                       + guid +
                       @"\4f971e89-eebd-4455-a8de-9e59040e7347",
                Section = "5ca83367-6e45-459f-a27b-476b1d01c936",
                Name = settingIndex,
            };
    }
}
