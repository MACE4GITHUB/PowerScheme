namespace RegistryManager
{
    using Microsoft.Win32;
    using Model;
    using System;
    using System.Collections.Generic;

    public static class Registry
    {
        public static string GetSettings(RegistryParam registryParam)
        {
            object regValue = null;
            using (var regKey = GetRegistryKey(registryParam))
            {
                if (regKey != null)
                {
                    regValue = registryParam.Name != null ? regKey.GetValue(registryParam.Name, registryParam.Value) : registryParam.Section;
                }
            }
            return regValue?.ToString();
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
            using (var regKey = GetRegistryKey(registryParam, true))
            {
                if (regKey == null) return;
                switch (registryParam.RegistryValueKind)
                {
                    case RegistryValueKind.String:
                    case RegistryValueKind.ExpandString:
                        regKey.SetValue(registryParam.Name, registryParam.Value, registryParam.RegistryValueKind);
                        break;
                    case RegistryValueKind.DWord:
                        if (!int.TryParse(registryParam.Value, out var resultInt))
                        {
                            return;
                        }
                        regKey.SetValue(registryParam.Name, resultInt, registryParam.RegistryValueKind);
                        break;
                    case RegistryValueKind.QWord:
                        if (!ulong.TryParse(registryParam.Value, out var resultUlong))
                        {
                            return;
                        }
                        regKey.SetValue(registryParam.Name, resultUlong, registryParam.RegistryValueKind);
                        break;
                    default:
                        return;
                }
            }
        }

        public static void DeleteSetting(RegistryParam registryParam)
        {
            using (var regKey = GetRegistryKey(registryParam, true))
            {
                regKey?.DeleteValue(registryParam.Name, false);
            }
        }

        private static RegistryKey GetRegistryKey(RegistryParam registryParam, bool writable = false)
        {
            RegistryKey regKey;
            switch (registryParam.RegistryHive)
            {
                case RegistryHive.ClassesRoot:
                    regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(registryParam.RegistrySubKey, writable);
                    break;
                case RegistryHive.CurrentUser:
                    regKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(registryParam.RegistrySubKey, writable);
                    break;
                case RegistryHive.LocalMachine:
                    regKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(registryParam.RegistrySubKey, writable);
                    break;
                case RegistryHive.Users:
                    regKey = Microsoft.Win32.Registry.Users.OpenSubKey(registryParam.RegistrySubKey, writable);
                    break;
                case RegistryHive.PerformanceData:
                    regKey = Microsoft.Win32.Registry.PerformanceData.OpenSubKey(registryParam.RegistrySubKey, writable);
                    break;
                case RegistryHive.CurrentConfig:
                    regKey = Microsoft.Win32.Registry.CurrentConfig.OpenSubKey(registryParam.RegistrySubKey, writable);
                    break;
                case RegistryHive.DynData:
                    regKey = Microsoft.Win32.Registry.DynData.OpenSubKey(registryParam.RegistrySubKey, writable);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(registryParam.RegistryHive), registryParam.RegistryHive, null);
            }

            return regKey;
        }
    }
}
