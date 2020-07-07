using Microsoft.Win32;
using PowerManagerAPI;
using PowerSchemes.Properties;
using PowerSchemes.Utility;
using RegistryManager;
using RegistryManager.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static RegistryManager.Registry;

namespace PowerSchemes.Services
{
    public class RegistryService
    {
        public static string GetActiveScheme()
        {
            return GetSettings(RegActivePowerScheme);
        }

        public static bool IsRunOnStartup =>
             GetSettings(RegRunOnStartup) != null;

        public static bool IsShowHibernateOption =>
            GetSettings(RegShowHibernateOption) == "1";

        public static bool IsShowSleepOption =>
            GetSettings(RegShowSleepOption) == "1";

        public static bool IsShowLockOption =>
            GetSettings(RegShowLockOption) == "1";


        public static void SetStartup(bool isStart)
        {
            var registryParam = RegStartup;

            if (isStart) SaveSetting(registryParam);
            else DeleteSetting(registryParam);
        }

        public static string GetFriendlyNamePowerScheme(Guid guid)
        {
            return GetSettings(RegFriendlyNamePowerSchemes(guid));
        }

        public static string GetDescriptionPowerScheme(Guid guid)
        {
            return GetSettings(RegDescriptionPowerSchemes(guid));
        }

        public static bool IsExistsTypicalPowerScheme(Guid guid)
            => GetSettings(RegTypicalPowerSchemes(guid)) != null;

        private static IEnumerable<string> DefaultPowerSchemes => GetSubKeys(RegDefaultPowerSchemes);

        private static IEnumerable<string> CurrentPowerSchemes => GetSubKeys(RegCurrentPowerSchemes);

        public static IEnumerable<string> UserPowerSchemes => CurrentPowerSchemes.Except(DefaultPowerSchemes).ToArray();

        public static RegistryWatcher ActivePowerSchemeRegistryWatcher()
            => new RegistryWatcher(RegActivePowerScheme)
            {
                RegChangeNotifyFilter = RegChangeNotifyFilter.Value
            };

        public static RegistryWatcher PowerSchemesRegistryWatcher()
            => new RegistryWatcher(RegCurrentPowerSchemes)
            {
                RegChangeNotifyFilter = RegChangeNotifyFilter.Key
            };

        public static void SetHibernateOption(string value)
        {
            SetRegistryValue(RegShowHibernateOption, value);
        }

        public static void SetSleepOption(string value)
        {
            SetRegistryValue(RegShowSleepOption, value);
        }

        private static void SetRegistryValue(RegistryParam registryParam, string value)
        {
            registryParam.Value = value;
            SaveRegistryEx(registryParam);
        }

        private static void SaveRegistryEx(RegistryParam registryParam)
        {
            var executorRegistryService = 
                new ExecutorRegistryService(
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
                RegistryValueKind = RegistryValueKind.String,
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
                RegistryValueKind = RegistryValueKind.String
            };

        private static RegistryParam RegDescriptionPowerSchemes(Guid guid) =>
            new RegistryParam()
            {
                RegistryHive = RegistryHive.LocalMachine,
                Path = @"SYSTEM\CurrentControlSet\Control\Power\User\PowerSchemes",
                Section = guid.ToString(),
                Name = "Description",
                RegistryValueKind = RegistryValueKind.String
            };

        private static RegistryParam RegTypicalPowerSchemes(Guid guid) =>
            new RegistryParam()
            {
                RegistryHive = RegistryHive.LocalMachine,
                Path = @"SYSTEM\CurrentControlSet\Control\Power\User\PowerSchemes",
                Section = guid.ToString()
            };

        private static RegistryParam RegRunOnStartup =>
            new RegistryParam()
            {
                RegistryHive = RegistryHive.CurrentUser,
                Path = @"SOFTWARE\Microsoft\Windows\CurrentVersion",
                Section = "Run",
                Name = "PowerScheme",
                RegistryValueKind = RegistryValueKind.String
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
                RegistryValueKind = RegistryValueKind.DWord
            };

        private static RegistryParam RegShowSleepOption =>
            new RegistryParam()
            {
                RegistryHive = RegistryHive.LocalMachine,
                Path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer",
                Section = "FlyoutMenuSettings",
                Name = "ShowSleepOption",
                RegistryValueKind = RegistryValueKind.DWord
            };

        private static RegistryParam RegShowLockOption =>
            new RegistryParam()
            {
                RegistryHive = RegistryHive.LocalMachine,
                Path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer",
                Section = "FlyoutMenuSettings",
                Name = "ShowLockOption",
                RegistryValueKind = RegistryValueKind.DWord
            };
    }
}
