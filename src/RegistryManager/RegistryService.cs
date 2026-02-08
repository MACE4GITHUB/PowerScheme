using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using Microsoft.Win32;
using PowerManagerAPI;
using RegistryManager.Model;
using static RegistryManager.Registry;

namespace RegistryManager;

public static class RegistryService
{
    public static bool IsRunOnStartup
    {
        get
        {
            var result = GetSettings<string>(RegRunOnStartup);
            if (result.IsError)
            {
                return false;
            }

            var isPathCorrect =
                result.Data?
                    .Equals(Paths.ApplicationFileName, StringComparison.InvariantCultureIgnoreCase)
                ?? false;

            if (!isPathCorrect)
            {
                SetStartup(true);
            }

            return true;
        }
    }

    public static bool IsShowHibernateOption
    {
        get
        {
            var result = GetSettings<int>(RegShowHibernateOption);
            if (result.IsError)
            {
                return false;
            }

            return result.Data == 1;
        }
    }

    public static bool IsShowSleepOption
    {
        get
        {
            var result = GetSettings<int>(RegShowSleepOption);
            if (result.IsError)
            {
                return true;
            }

            return result.Data == 1;
        }
    }

    public static bool IsShowLockOption
    {
        get
        {
            var result = GetSettings<int>(RegShowLockOption);
            if (result.IsError)
            {
                return true;
            }

            return result.Data == 1;
        }
    }


    public static void SetStartup(bool isStart)
    {
        var registryParam = RegStartup;

        if (isStart)
        {
            SaveSetting(registryParam);
        }
        else
        {
            DeleteSetting(registryParam);
        }
    }

    public static int GetLidOption(Guid guid)
    {
        var result = GetSettings<int>(RegLidOption(guid));
        return result.IsError ? 1 : result.Data;
    }

    public static bool IsFirstStart(string company, string product)
    {
        var result = GetSettings<int>(RegAppSettings(company, product));
        if (result.IsError)
        {
            return true;
        }

        return result.Data != 0;
    }

    public static bool ExistsTypicalPowerScheme(Guid guid)
    {
        var result = GetSettings<string>(RegPowerSchemes(guid));
        return result.IsSuccess;
    }

    public static bool ExistsDefaultPowerScheme(Guid guid)
    {
        var result = GetSettings<string>(RegPowerSchemes(guid, true));
        return result.IsSuccess;
    }

    private static ICollection<Guid> DefaultPowerSchemes =>
        GetGuidSubKeys(RegPowerSchemes(true));

    private static ICollection<Guid> CurrentPowerSchemes =>
        GetGuidSubKeys(RegPowerSchemes());

    public static ICollection<Guid> UserPowerSchemes =>
        [.. CurrentPowerSchemes.Except(DefaultPowerSchemes)];

    public static RegistryWatcher<string> ActivePowerSchemeRegistryWatcher()
        => new(RegActivePowerScheme)
        {
            RegChangeNotifyFilter = RegChangeNotifyFilter.Value
        };

    public static RegistryWatcher<string> PowerSchemesRegistryWatcher()
        => new(RegPowerSchemes())
        {
            RegChangeNotifyFilter = RegChangeNotifyFilter.Key
        };

    public static void SetHibernateOption(ResourceManager resourceManager, object value) =>
        SetRegistryValue(resourceManager, RegShowHibernateOption, value);

    public static void SetSleepOption(ResourceManager resourceManager, object value) =>
        SetRegistryValue(resourceManager, RegShowSleepOption, value);

    public static void SetAppSettings(string company, string product, object value)
    {
        var regAppSettings = RegAppSettings(company, product);
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
            new RegistryExecutorService(
                resourceManager,
                registryParam,
                RegistryAdminAction.Set,
                Guid.NewGuid());

        executorRegistryService.Execute();
    }

    private static RegistryParam RegActivePowerScheme =>
        new()
        {
            RegistryHive = RegistryHive.LocalMachine,
            Path = @"SYSTEM\CurrentControlSet\Control\Power\User",
            Section = "PowerSchemes",
            Name = "ActivePowerScheme",
            Value = PowerManager.GetActivePlan().ToString()
        };

    private static RegistryParam RegPowerSchemes(bool isDefault = false) =>
        new()
        {
            RegistryHive = RegistryHive.LocalMachine,
            Path = @"SYSTEM\CurrentControlSet\Control\Power\User" +
                   (isDefault ? @"\Default" : "") +
                   @"\PowerSchemes"
        };

    private static RegistryParam RegPowerSchemes(Guid guid, bool isDefault = false)
    {
        var regPowerSchemes = RegPowerSchemes(isDefault);
        regPowerSchemes.Section = guid.ToString();
        return regPowerSchemes;
    }

    private static RegistryParam RegRunOnStartup =>
        new()
        {
            RegistryHive = RegistryHive.CurrentUser,
            Path = @"SOFTWARE\Microsoft\Windows\CurrentVersion",
            Section = "Run",
            Name = "PowerScheme",
        };

    private static RegistryParam RegAppSettings(string company, string product) =>
        new()
        {
            RegistryHive = RegistryHive.CurrentUser,
            Path = @"SOFTWARE\" + company,
            Section = product,
            Name = "ShowDialogFirstStart"
        };

    private static RegistryParam RegStartup
    {
        get
        {
            var rp = RegRunOnStartup;
            rp.Value = Paths.ApplicationFileName;
            return rp;
        }
    }

    private static RegistryParam RegShowHibernateOption =>
        new()
        {
            RegistryHive = RegistryHive.LocalMachine,
            Path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer",
            Section = "FlyoutMenuSettings",
            Name = "ShowHibernateOption",
        };

    private static RegistryParam RegShowSleepOption =>
        new()
        {
            RegistryHive = RegistryHive.LocalMachine,
            Path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer",
            Section = "FlyoutMenuSettings",
            Name = "ShowSleepOption",
        };

    private static RegistryParam RegShowLockOption =>
        new()
        {
            RegistryHive = RegistryHive.LocalMachine,
            Path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer",
            Section = "FlyoutMenuSettings",
            Name = "ShowLockOption",
        };

    private static RegistryParam RegLidOption(Guid guid, string settingIndex = "ACSettingIndex") =>
        new()
        {
            RegistryHive = RegistryHive.LocalMachine,
            Path = @"SYSTEM\CurrentControlSet\Control\Power\User\PowerSchemes\"
                   + guid +
                   @"\4f971e89-eebd-4455-a8de-9e59040e7347",
            Section = "5ca83367-6e45-459f-a27b-476b1d01c936",
            Name = settingIndex,
        };
}