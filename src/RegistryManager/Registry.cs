using Common;

namespace RegistryManager;

using Microsoft.Win32;
using Model;
using System;
using System.Collections.Generic;

public static class Registry
{
    public static Result<T> GetSettings<T>(RegistryParam registryParam)
    {
        T regValue;
        using (var regKey = GetRegistryKey(registryParam))
        {
            if (regKey == null) return Result<T>.Fail<T>("Registry Key is not found");

            if (registryParam.Name != null)
            {
                var value = regKey.GetValue(registryParam.Name, registryParam.Value);

                if (value == null) return Result<T>.Fail<T>("Registry Parameter is not found");

                regValue = (T)value;
            }
            else
            {
                var type = typeof(T).Name.ToLowerInvariant() == "string";
                regValue = (T)(object)registryParam.Section;
            }
        }
        return Result<T>.Ok(regValue);
    }

    public static IEnumerable<string> GetSubKeys(RegistryParam registryParam)
    {
        string[] regValue = { };
        using (var regKey = GetRegistryKey(registryParam))
        {
            if (regKey != null)
            {
                regValue = regKey.GetSubKeyNames();
            }
        }
        return regValue;
    }

    public static void SaveSetting(RegistryParam registryParam)
    {
        using (var regKey = GetRegistryKey(registryParam, true, true))
        {
            if (regKey == null) return;

            regKey.SetValue(registryParam.Name, registryParam.Value, registryParam.RegistryValueKind);
        }
    }

    public static void DeleteSetting(RegistryParam registryParam)
    {
        using (var regKey = GetRegistryKey(registryParam, true))
        {
            regKey?.DeleteValue(registryParam.Name, false);
        }
    }

    public static bool ExistsSettings(RegistryParam registryParam)
    {
        using (var regKey = GetRegistryKey(registryParam))
        {
            if (regKey == null) return false;
            if (registryParam.Name == null) return true;
            var regValue = regKey.GetValue(registryParam.Name, registryParam.Value);
            return regValue != null;
        }
    }

    private static RegistryKey GetRegistryKey(RegistryParam registryParam, bool writable = false, bool create = false)
    {
        RegistryKey regKey;
        switch (registryParam.RegistryHive)
        {
            case RegistryHive.ClassesRoot:
                regKey = create
                    ? Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(registryParam.RegistrySubKey, writable)
                    : Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(registryParam.RegistrySubKey, writable);
                break;
            case RegistryHive.CurrentUser:
                regKey = create
                    ? Microsoft.Win32.Registry.CurrentUser.CreateSubKey(registryParam.RegistrySubKey, writable)
                    : Microsoft.Win32.Registry.CurrentUser.OpenSubKey(registryParam.RegistrySubKey, writable);
                break;
            case RegistryHive.LocalMachine:
                regKey = RegistryKeyLocalMachine(registryParam, writable, create);
                break;
            case RegistryHive.Users:
                regKey = create
                    ? Microsoft.Win32.Registry.Users.CreateSubKey(registryParam.RegistrySubKey, writable)
                    : Microsoft.Win32.Registry.Users.OpenSubKey(registryParam.RegistrySubKey, writable);
                break;
            case RegistryHive.PerformanceData:
                regKey = create
                    ? Microsoft.Win32.Registry.PerformanceData.CreateSubKey(registryParam.RegistrySubKey, writable)
                    : Microsoft.Win32.Registry.PerformanceData.OpenSubKey(registryParam.RegistrySubKey, writable);
                break;
            case RegistryHive.CurrentConfig:
                regKey = create
                    ? Microsoft.Win32.Registry.CurrentConfig.CreateSubKey(registryParam.RegistrySubKey, writable)
                    : Microsoft.Win32.Registry.CurrentConfig.OpenSubKey(registryParam.RegistrySubKey, writable);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(registryParam.RegistryHive), registryParam.RegistryHive, null);
        }

        return regKey;
    }

    private static RegistryKey RegistryKeyLocalMachine(RegistryParam registryParam, bool writable, bool create)
    {
        var regKey = RegistryKeyLocalMachineX(registryParam, writable, create, RegistryView.Registry64);

        if (regKey != null) return regKey;

        regKey = RegistryKeyLocalMachineX(registryParam, writable, create, RegistryView.Registry32);

        return regKey;
    }

    private static RegistryKey RegistryKeyLocalMachineX(RegistryParam registryParam, bool writable, bool create, RegistryView registryView)
    {
        var localMachineKey =
            RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView);

        return create
            ? localMachineKey.CreateSubKey(registryParam.RegistrySubKey, writable)
            : localMachineKey.OpenSubKey(registryParam.RegistrySubKey, writable);
    }
}