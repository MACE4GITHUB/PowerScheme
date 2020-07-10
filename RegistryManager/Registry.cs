namespace RegistryManager
{
    using Microsoft.Win32;
    using Model;
    using System;
    using System.Collections.Generic;

    public static class Registry
    {
        public static T GetSettings<T>(RegistryParam registryParam)
        {
            T regValue = default;
            using (var regKey = GetRegistryKey(registryParam))
            {
                if (regKey == null) return regValue;

                if (registryParam.Name != null)
                    regValue = (T) regKey.GetValue(registryParam.Name, registryParam.Value);
                else
                {
                    var type = typeof(T).Name.ToLowerInvariant() == "string";
                    regValue = (T)(object)registryParam.Section;
                }
            }
            return regValue;
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
                    regKey = create
                        ? Microsoft.Win32.Registry.LocalMachine.CreateSubKey(registryParam.RegistrySubKey, writable)
                        : Microsoft.Win32.Registry.LocalMachine.OpenSubKey(registryParam.RegistrySubKey, writable);
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
                case RegistryHive.DynData:
                    regKey = create
#pragma warning disable CS0618 
                        ? Microsoft.Win32.Registry.DynData.CreateSubKey(registryParam.RegistrySubKey, writable)
                        : Microsoft.Win32.Registry.DynData.OpenSubKey(registryParam.RegistrySubKey, writable);
#pragma warning restore CS0618 
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(registryParam.RegistryHive), registryParam.RegistryHive, null);
            }

            return regKey;
        }
    }
}
