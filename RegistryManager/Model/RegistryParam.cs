using System;
using System.ComponentModel;
using System.Text;
using Microsoft.Win32;

namespace RegistryManager.Model
{
    [Serializable]
    public struct RegistryParam
    {
        private const string HKEY_CLASSES_ROOT = "HKEY_CLASSES_ROOT";
        private const string HKEY_CURRENT_CONFIG = "HKEY_CURRENT_CONFIG";
        private const string HKEY_CURRENT_USER = "HKEY_CURRENT_USER";
        private const string HKEY_DYN_DATA = "HKEY_DYN_DATA";
        private const string HKEY_LOCAL_MACHINE = "HKEY_LOCAL_MACHINE";
        private const string HKEY_PERFORMANCE_DATA = "HKEY_PERFORMANCE_DATA";
        private const string HKEY_USERS = "HKEY_USERS";

        private const string DELIMITER = "|&|";

        private RegistryValueKind _registryValueKind;

        public RegistryHive RegistryHive { get; set; }

        public string Path { get; set; }

        public string Section { get; set; }

        public string Name { get; set; }

        public RegistryValueKind RegistryValueKind
        {
            get => _registryValueKind == RegistryValueKind.Unknown ? CalculateValueKind() : _registryValueKind;
            set => _registryValueKind = value;
        }

        public object Value { get; set; }


        /// <summary>
        /// Returns 'Path + "\\" + Section'
        /// </summary>
        public string RegistrySubKey => Path + "\\" + Section;

        public string Hive => CalculateRegistryHive();

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

            var registryValueKind = (RegistryValueKind) Enum.Parse(typeof(RegistryValueKind), param[4]);
            var value = CalculateTypeValue(param[5], registryValueKind);
            var registryParam = new RegistryParam
            {
                RegistryHive = (RegistryHive)Enum.Parse(typeof(RegistryHive), param[0]),
                Path = param[1],
                Section = param[2],
                Name = param[3],
                RegistryValueKind = registryValueKind,
                Value = value
            };

            return registryParam;
        }

        private RegistryValueKind CalculateValueKind()
        {
            switch (Value)
            {
                case null:
                    return RegistryValueKind.None;
                case int _:
                    return RegistryValueKind.DWord;
                case uint _:
                    return RegistryValueKind.QWord;
                case Array _:
                    switch (Value)
                    {
                        case byte[] _:
                            return RegistryValueKind.Binary;
                        case string[] _:
                            return RegistryValueKind.MultiString;
                        default:
                            throw new ArgumentException(Value.GetType().Name);
                    }
                default:
                    return RegistryValueKind.String;
            }
        }

        private static object CalculateTypeValue(string value, RegistryValueKind registryValueKind)
        {
            switch (registryValueKind)
            {
                case RegistryValueKind.String:
                    return value;
                case RegistryValueKind.ExpandString:
                    break;
                case RegistryValueKind.Binary:
                    break;
                case RegistryValueKind.DWord:
                    return int.Parse(value);
                case RegistryValueKind.MultiString:
                    break;
                case RegistryValueKind.QWord:
                    return uint.Parse(value);
                case RegistryValueKind.Unknown:
                    break;
                case RegistryValueKind.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(registryValueKind), registryValueKind, null);
            }

            return value;
        }

        private string CalculateRegistryHive()
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
}
