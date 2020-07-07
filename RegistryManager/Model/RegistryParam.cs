using System;
using System.ComponentModel;
using System.Text;
using Microsoft.Win32;

namespace RegistryManager.Model
{
    public struct RegistryParam
    {
        private const string HKEY_CLASSES_ROOT = "HKEY_CLASSES_ROOT";
        private const string HKEY_CURRENT_CONFIG = "HKEY_CURRENT_CONFIG";
        private const string HKEY_CURRENT_USER = "HKEY_CURRENT_USER";
        private const string HKEY_DYN_DATA = "HKEY_DYN_DATA";
        private const string HKEY_LOCAL_MACHINE = "HKEY_LOCAL_MACHINE";
        private const string HKEY_PERFORMANCE_DATA = "HKEY_PERFORMANCE_DATA";
        private const string HKEY_USERS = "HKEY_USERS";

        private const string DELIMITER = "|+&+|";


        public RegistryHive RegistryHive { get; set; }

        public string Path { get; set; }

        public string Section { get; set; }

        public string Name { get; set; }

        public RegistryValueKind RegistryValueKind { get; set; }

        public string Value { get; set; }

        
        /// <summary>
        /// Returns 'Path + "\\" + Section'
        /// </summary>
        public string RegistrySubKey => Path + "\\" + Section;

        public string Hive
        {
            get
            {
                switch (RegistryHive)
                {
                    case RegistryHive.ClassesRoot:
                        return HKEY_CLASSES_ROOT;

                    case RegistryHive.CurrentConfig:
                        return HKEY_CURRENT_CONFIG;

                    case RegistryHive.CurrentUser:
                        return HKEY_CURRENT_USER;

                    case RegistryHive.DynData:
                        return HKEY_DYN_DATA;

                    case RegistryHive.LocalMachine:
                        return HKEY_LOCAL_MACHINE;

                    case RegistryHive.PerformanceData:
                        return HKEY_PERFORMANCE_DATA;

                    case RegistryHive.Users:
                        return HKEY_USERS;

                    default:
                        throw new InvalidEnumArgumentException(nameof(RegistryHive),
                            (int)RegistryHive, typeof(RegistryHive));
                }
            }
        }

        /// <summary>
        /// Returns Hive + "\\" + RegistrySubKey
        /// </summary>
        public string RegistryKey => Hive + "\\" + RegistrySubKey;

        public override string ToString()
        {
            return $"{RegistryHive}{DELIMITER}{Path}{DELIMITER}{Section}{DELIMITER}{Name}{DELIMITER}{RegistryValueKind}{DELIMITER}{Value}";
        }

        public static RegistryParam FromString(string v)
        {
            var param = v.SplitByString(DELIMITER);

            if (param.Length < 5)
            {
                return new RegistryParam();
            }

            var registryParam = new RegistryParam()
            {
                RegistryHive = (RegistryHive)Enum.Parse(typeof(RegistryHive), param[0]),
                Path = param[1],
                Section = param[2],
                Name = param[3],
                RegistryValueKind = (RegistryValueKind)Enum.Parse(typeof(RegistryValueKind), param[4]),
                Value = param[5]
            };

            return registryParam;
        }

    }
}
