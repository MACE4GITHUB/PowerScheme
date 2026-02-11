using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using RegistryManager.Common;

namespace RegistryManager.Api;

public static class Registry
{
    public static Result<T> GetSettings<T>(RegistryParam registryParam)
    {
        using var regKey = GetRegistryKey(registryParam);

        if (regKey == null)
        {
            return Result<T>.Fail<T>("Registry Key is not found");
        }

        if (registryParam.Name == null)
        {
            return Result<T>.Ok((T)(object)registryParam.Section);
        }

        var value = regKey.GetValue(registryParam.Name, registryParam.Value);

        return value != null!
            ? Result<T>.Ok((T)value)
            : Result<T>.Fail<T>("Registry Parameter is not found");
    }

    public static ICollection<string> GetSubKeys(RegistryParam registryParam)
    {
        using var regKey = GetRegistryKey(registryParam);

        return regKey != null
            ? regKey.GetSubKeyNames()
            : [];
    }

    public static ICollection<Guid> GetGuidSubKeys(RegistryParam registryParam)
    {
        var subKeys = GetSubKeys(registryParam);

        if (subKeys.Count == 0)
        {
            return [];
        }

        return
        [
            .. subKeys
                .Select(x => Guid.TryParse(x, out var guid)
                    ? guid
                    : Guid.Empty)
                .Where(x => x != Guid.Empty)
        ];
    }

    public static void SaveSetting(RegistryParam registryParam)
    {
        if (registryParam.Value == null)
        {
            throw new ArgumentException(nameof(registryParam.Value));
        }

        using var regKey = GetRegistryKey(registryParam, true, true);

        regKey?.SetValue(registryParam.Name, registryParam.Value, registryParam.RegistryValueKind);
    }

    public static void DeleteSetting(RegistryParam registryParam)
    {
        using var regKey = GetRegistryKey(registryParam, true);

        if (registryParam.Name != null)
        {
            regKey?.DeleteValue(registryParam.Name, false);
        }
    }

    public static bool ExistsSettings(RegistryParam registryParam)
    {
        using var regKey = GetRegistryKey(registryParam);

        if (regKey == null)
        {
            return false;
        }

        if (registryParam.Name == null)
        {
            return true;
        }

        var regValue = regKey.GetValue(registryParam.Name, registryParam.Value);

        return regValue != null!;
    }

    private static RegistryKey? GetRegistryKey(
        RegistryParam registryParam,
        bool writable = false,
        bool create = false) =>
        registryParam.RegistryHive switch
        {
            RegistryHive.ClassesRoot => create
                ? Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(registryParam.RegistrySubKey, writable)
                : Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(registryParam.RegistrySubKey, writable),
            RegistryHive.CurrentUser => create
                ? Microsoft.Win32.Registry.CurrentUser.CreateSubKey(registryParam.RegistrySubKey, writable)
                : Microsoft.Win32.Registry.CurrentUser.OpenSubKey(registryParam.RegistrySubKey, writable),
            RegistryHive.LocalMachine => RegistryKeyLocalMachine(registryParam, writable, create),
            RegistryHive.Users => create
                ? Microsoft.Win32.Registry.Users.CreateSubKey(registryParam.RegistrySubKey, writable)
                : Microsoft.Win32.Registry.Users.OpenSubKey(registryParam.RegistrySubKey, writable),
            RegistryHive.PerformanceData => create
                ? Microsoft.Win32.Registry.PerformanceData.CreateSubKey(registryParam.RegistrySubKey, writable)
                : Microsoft.Win32.Registry.PerformanceData.OpenSubKey(registryParam.RegistrySubKey, writable),
            RegistryHive.CurrentConfig => create
                ? Microsoft.Win32.Registry.CurrentConfig.CreateSubKey(registryParam.RegistrySubKey, writable)
                : Microsoft.Win32.Registry.CurrentConfig.OpenSubKey(registryParam.RegistrySubKey, writable),
            _ => null
        };

    private static RegistryKey? RegistryKeyLocalMachine(
        RegistryParam registryParam,
        bool writable,
        bool create)
    {
        var regKey = RegistryKeyLocalMachineX(registryParam, writable, create, RegistryView.Registry64);

        if (regKey != null)
        {
            return regKey;
        }

        regKey = RegistryKeyLocalMachineX(registryParam, writable, create, RegistryView.Registry32);

        return regKey;
    }

    private static RegistryKey? RegistryKeyLocalMachineX(
        RegistryParam registryParam,
        bool writable,
        bool create,
        RegistryView registryView)
    {
        var localMachineKey =
            RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView);

        return create
            ? localMachineKey.CreateSubKey(registryParam.RegistrySubKey, writable)
            : localMachineKey.OpenSubKey(registryParam.RegistrySubKey, writable);
    }
}
