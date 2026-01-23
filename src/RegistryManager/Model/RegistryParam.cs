using System;
using System.ComponentModel;
using Microsoft.Win32;
using RegistryManager.Extensions;

namespace RegistryManager.Model;

public struct RegistryParam
{
    private const string HKEY_CLASSES_ROOT = "HKEY_CLASSES_ROOT";
    private const string HKEY_CURRENT_CONFIG = "HKEY_CURRENT_CONFIG";
    private const string HKEY_CURRENT_USER = "HKEY_CURRENT_USER";
    private const string HKEY_LOCAL_MACHINE = "HKEY_LOCAL_MACHINE";
    private const string HKEY_PERFORMANCE_DATA = "HKEY_PERFORMANCE_DATA";
    private const string HKEY_USERS = "HKEY_USERS";

    private const string DELIMITER = "|&|";
    private const string DELIMITER_STRINGS = "|s|";

    public RegistryHive RegistryHive { get; set; }

    public string Path { get; set; }

    public string Section { get; set; }

    public string? Name { get; set; }

    public RegistryValueKind RegistryValueKind
    {
        readonly get => field == RegistryValueKind.Unknown
            ? CalculateValueKind()
            : field;
        set;
    }

    public object? Value { get; set; }


    /// <summary>
    /// Returns 'Path + "\\" + Section'
    /// </summary>
    public readonly string RegistrySubKey => Path + "\\" + Section;

    public readonly string Hive => CalculateRegistryHive();

    /// <summary>
    /// Returns Hive + "\\" + RegistrySubKey
    /// </summary>
    public readonly string RegistryKey => Hive + "\\" + RegistrySubKey;

    public override readonly string ToString()
    {
        var value = CalculateStringValue(Value!, RegistryValueKind);
        return $"{RegistryHive}{DELIMITER}{Path}{DELIMITER}{Section}{DELIMITER}{Name}{DELIMITER}{RegistryValueKind}{DELIMITER}{value}";
    }

    public static RegistryParam FromString(string v)
    {
        var param = v.SplitByString(DELIMITER);

        if (param.Length < 5)
        {
            return new RegistryParam();
        }

        var registryValueKind = (RegistryValueKind)Enum.Parse(typeof(RegistryValueKind), param[4], true);
        var value = CalculateTypedValue(param[5], registryValueKind);

        var registryParam = new RegistryParam
        {
            RegistryHive = (RegistryHive)Enum.Parse(typeof(RegistryHive), param[0], true),
            Path = param[1],
            Section = param[2],
            Name = param[3],
            RegistryValueKind = registryValueKind,
            Value = value
        };

        return registryParam;
    }

    private readonly RegistryValueKind CalculateValueKind() =>
        Value switch
        {
            null => RegistryValueKind.None,
            int _ => RegistryValueKind.DWord,
            uint _ => RegistryValueKind.QWord,
            Array _ => Value switch
            {
                byte[] _ => RegistryValueKind.Binary,
                string[] _ => RegistryValueKind.MultiString,
                _ => throw new ArgumentException(Value.GetType().Name)
            },
            _ => RegistryValueKind.String
        };

    private static object CalculateTypedValue(string value, RegistryValueKind registryValueKind) =>
        registryValueKind switch
        {
            RegistryValueKind.String or RegistryValueKind.ExpandString => value,
            RegistryValueKind.DWord => int.Parse(value),
            RegistryValueKind.QWord => uint.Parse(value),
            RegistryValueKind.Binary => value.DecodeToBytes(),
            RegistryValueKind.MultiString => value.SplitByString(DELIMITER_STRINGS),
            _ => throw new ArgumentOutOfRangeException(nameof(registryValueKind), registryValueKind, null)
        };

    private static string CalculateStringValue(object value, RegistryValueKind registryValueKind) =>
        registryValueKind switch
        {
            RegistryValueKind.String or
                RegistryValueKind.ExpandString or
                RegistryValueKind.DWord or
                RegistryValueKind.QWord => $"{value}",
            RegistryValueKind.Binary => ((byte[])value).EncodeToString(),
            RegistryValueKind.MultiString => string.Join(DELIMITER_STRINGS, (string[])value),
            _ => throw new ArgumentOutOfRangeException(nameof(registryValueKind), registryValueKind, null)
        };

    private readonly string CalculateRegistryHive() =>
        RegistryHive switch
        {
            RegistryHive.ClassesRoot => HKEY_CLASSES_ROOT,
            RegistryHive.CurrentConfig => HKEY_CURRENT_CONFIG,
            RegistryHive.CurrentUser => HKEY_CURRENT_USER,
            RegistryHive.LocalMachine => HKEY_LOCAL_MACHINE,
            RegistryHive.PerformanceData => HKEY_PERFORMANCE_DATA,
            RegistryHive.Users => HKEY_USERS,
            _ => throw new InvalidEnumArgumentException(nameof(RegistryHive), (int)RegistryHive, typeof(RegistryHive))
        };
}
