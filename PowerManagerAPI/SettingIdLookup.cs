using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace PowerManagerAPI
{
    public enum ErrorCode : uint {
        SUCCESS                 = 0x000,
        FILE_NOT_FOUND          = 0x002,
        ERROR_INVALID_PARAMETER = 0x057,
        ERROR_ALREADY_EXISTS    = 0x0B7,
        MORE_DATA               = 0x0EA,
        NO_MORE_ITEMS           = 0x103
    }

    public enum AccessFlags : uint
    {
        ACCESS_SCHEME = 16,
        ACCESS_SUBGROUP = 17,
        ACCESS_INDIVIDUAL_SETTING = 18
    }

    public enum SettingSubgroup
    {
        NO_SUBGROUP,
        DISK_SUBGROUP,
        SYSTEM_BUTTON_SUBGROUP,
        PROCESSOR_SETTINGS_SUBGROUP,
        VIDEO_SUBGROUP,
        BATTERY_SUBGROUP,
        SLEEP_SUBGROUP,
        PCIEXPRESS_SETTINGS_SUBGROUP
    }

    public enum Setting
    {
        BATACTIONCRIT,
        BATACTIONLOW,
        BATFLAGSLOW,
        BATLEVELCRIT,
        BATLEVELLOW,
        LIDACTION,
        PBUTTONACTION,
        SBUTTONACTION,
        UIBUTTON_ACTION,
        DISKIDLE,
        ASPM,
        PROCFREQMAX,
        PROCTHROTTLEMAX,
        PROCTHROTTLEMIN,
        SYSCOOLPOL,
        HIBERNATEIDLE,
        HYBRIDSLEEP,
        RTCWAKE,
        STANDBYIDLE,
        ADAPTBRIGHT,
        VIDEOIDLE
    }

    public enum PowerPlatformRole
    {
        /// <summary>The OEM did not specify a specific role.</summary>
        PlatformRoleUnspecified,

        /// <summary>The OEM specified a desktop role.</summary>
        PlatformRoleDesktop,

        /// <summary>The OEM specified a mobile role (for example, a laptop).</summary>
        PlatformRoleMobile,

        /// <summary>The OEM specified a workstation role.</summary>
        PlatformRoleWorkstation,

        /// <summary>The OEM specified an enterprise server role.</summary>
        PlatformRoleEnterpriseServer,

        /// <summary>The OEM specified a single office/home office (SOHO) server role.</summary>
        PlatformRoleSOHOServer,

        /// <summary>The OEM specified an appliance PC role.</summary>
        PlatformRoleAppliancePC,

        /// <summary>The OEM specified a performance server role.</summary>
        PlatformRolePerformanceServer,

        /// <summary>
        /// The OEM specified a tablet form factor role. Windows 7, Windows Server 2008 R2, Windows Vista or Windows Server 2008: In
        /// version 1 of this enumeration, this value is equivalent to PlatformRoleMaximum. This value is supported in version 2 of this
        /// enumeration starting with Windows 8 and Windows Server 2012.
        /// </summary>
        PlatformRoleSlate,

        /// <summary>Values equal to or greater than this value indicate an out of range value.</summary>
        PlatformRoleMaximum,
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct SYSTEM_POWER_CAPABILITIES
    {
        /// <summary>If this member is <c>TRUE</c>, there is a system power button.</summary>
        [MarshalAs(UnmanagedType.U1)] public bool PowerButtonPresent;

        /// <summary>If this member is <c>TRUE</c>, there is a system sleep button.</summary>
        [MarshalAs(UnmanagedType.U1)] public bool SleepButtonPresent;

        /// <summary>If this member is <c>TRUE</c>, there is a lid switch.</summary>
        [MarshalAs(UnmanagedType.U1)] public bool LidPresent;

        /// <summary>If this member is <c>TRUE</c>, the operating system supports sleep state S1.</summary>
        [MarshalAs(UnmanagedType.U1)] public bool SystemS1;

        /// <summary>If this member is <c>TRUE</c>, the operating system supports sleep state S2.</summary>
        [MarshalAs(UnmanagedType.U1)] public bool SystemS2;

        /// <summary>If this member is <c>TRUE</c>, the operating system supports sleep state S3.</summary>
        [MarshalAs(UnmanagedType.U1)] public bool SystemS3;

        /// <summary>If this member is <c>TRUE</c>, the operating system supports sleep state S4 (hibernation).</summary>
        [MarshalAs(UnmanagedType.U1)] public bool SystemS4;

        /// <summary>If this member is <c>TRUE</c>, the operating system supports power off state S5 (soft off).</summary>
        [MarshalAs(UnmanagedType.U1)] public bool SystemS5;

        /// <summary>If this member is <c>TRUE</c>, the system hibernation file is present.</summary>
        [MarshalAs(UnmanagedType.U1)] public bool HiberFilePresent;

        /// <summary>If this member is <c>TRUE</c>, the system supports wake capabilities.</summary>
        [MarshalAs(UnmanagedType.U1)] public bool FullWake;

        /// <summary>If this member is <c>TRUE</c>, the system supports video display dimming capabilities.</summary>
        [MarshalAs(UnmanagedType.U1)] public bool VideoDimPresent;

        /// <summary>If this member is <c>TRUE</c>, the system supports APM BIOS power management features.</summary>
        [MarshalAs(UnmanagedType.U1)] public bool ApmPresent;

        /// <summary>If this member is <c>TRUE</c>, there is an uninterruptible power supply (UPS).</summary>
        [MarshalAs(UnmanagedType.U1)] public bool UpsPresent;

        /// <summary>If this member is <c>TRUE</c>, the system supports thermal zones.</summary>
        [MarshalAs(UnmanagedType.U1)] public bool ThermalControl;

        /// <summary>If this member is <c>TRUE</c>, the system supports processor throttling.</summary>
        [MarshalAs(UnmanagedType.U1)] public bool ProcessorThrottle;

        /// <summary>The minimum level of system processor throttling supported, expressed as a percentage.</summary>
        public byte ProcessorMinThrottle;

        /// <summary>The processor throttle scale</summary>
        public byte ProcessorThrottleScale;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        private byte[] spare2;

        /// <summary>The maximum level of system processor throttling supported, expressed as a percentage.</summary>
        public byte ProcessorMaxThrottle;

        /// <summary>If this member is <c>TRUE</c>, the system supports the hybrid sleep state.</summary>
        [MarshalAs(UnmanagedType.U1)] public bool FastSystemS4;

        /// <summary>The hiberboot</summary>
        [MarshalAs(UnmanagedType.U1)] public bool Hiberboot;

        /// <summary>
        /// If this member is <c>TRUE</c>, the platform has support for ACPI wake alarm devices. For more details on wake alarm devices,
        /// please see the ACPI specification section 9.18.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)] public bool WakeAlarmPresent;

        /// <summary>If this member is <c>TRUE</c>, the system supports the S0 low power idle model.</summary>
        [MarshalAs(UnmanagedType.U1)] public bool AoAc;

        /// <summary>If this member is <c>TRUE</c>, the system supports allowing the removal of power to fixed disk devices.</summary>
        [MarshalAs(UnmanagedType.U1)] public bool DiskSpinDown;

        /// <summary>The hiber file type</summary>
        public byte HiberFileType;

        /// <summary>The ao ac connectivity supported</summary>
        [MarshalAs(UnmanagedType.U1)] public bool AoAcConnectivitySupported;

        /// <summary>The spare3</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        private readonly byte[] spare3;

        /// <summary>If this member is <c>TRUE</c>, there are one or more batteries in the system.</summary>
        [MarshalAs(UnmanagedType.U1)] public bool SystemBatteriesPresent;

        /// <summary>
        /// If this member is <c>TRUE</c>, the system batteries are short-term. Short-term batteries are used in uninterruptible power
        /// supplies (UPS).
        /// </summary>
        [MarshalAs(UnmanagedType.U1)] public bool BatteriesAreShortTerm;

        /// <summary>A BATTERY_REPORTING_SCALE structure that contains information about how system battery metrics are reported.</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)] public BATTERY_REPORTING_SCALE[] BatteryScale;

        /// <summary>
        /// The lowest system sleep state (Sx) that will generate a wake event when the system is on AC power. This member must be one of
        /// the SYSTEM_POWER_STATE enumeration type values.
        /// </summary>
        public SYSTEM_POWER_STATE AcOnLineWake;

        /// <summary>
        /// The lowest system sleep state (Sx) that will generate a wake event via the lid switch. This member must be one of the
        /// SYSTEM_POWER_STATE enumeration type values.
        /// </summary>
        public SYSTEM_POWER_STATE SoftLidWake;

        /// <summary>
        /// <para>
        /// The lowest system sleep state (Sx) supported by hardware that will generate a wake event via the Real Time Clock (RTC). This
        /// member must be one of the SYSTEM_POWER_STATE enumeration type values.
        /// </para>
        /// <para>
        /// To wake the computer using the RTC, the operating system must also support waking from the sleep state the computer is in
        /// when the RTC generates the wake event. Therefore, the effective lowest sleep state from which an RTC wake event can wake the
        /// computer is the lowest sleep state supported by the operating system that is equal to or higher than the value of
        /// <c>RtcWake</c>. To determine the sleep states that the operating system supports, check the <c>SystemS1</c>, <c>SystemS2</c>,
        /// <c>SystemS3</c>, and <c>SystemS4</c> members.
        /// </para>
        /// </summary>
        public SYSTEM_POWER_STATE RtcWake;

        /// <summary>
        /// The minimum allowable system power state supporting wake events. This member must be one of the SYSTEM_POWER_STATE
        /// enumeration type values. Note that this state may change as different device drivers are installed on the system.
        /// </summary>
        public SYSTEM_POWER_STATE MinDeviceWakeState;

        /// <summary>
        /// The default system power state used if an application calls RequestWakeupLatency with <c>LT_LOWEST_LATENCY</c>. This member
        /// must be one of the SYSTEM_POWER_STATE enumeration type values.
        /// </summary>
        public SYSTEM_POWER_STATE DefaultLowLatencyWake;
    }

    public enum SYSTEM_POWER_STATE
    {
        /// <summary>Indicates an unspecified system power state.</summary>
        PowerSystemUnspecified,

        /// <summary>Indicates maximum system power, which corresponds to system working state S0.</summary>
        PowerSystemWorking,

        /// <summary>
        /// Indicates a system sleeping state less than PowerSystemWorking and greater than PowerSystemSleeping2, which corresponds to
        /// system power state S1.
        /// </summary>
        PowerSystemSleeping1,

        /// <summary>
        /// Indicates a system sleeping state less than PowerSystemSleeping1 and greater than PowerSystemSleeping3, which corresponds to
        /// system power state S2.
        /// </summary>
        PowerSystemSleeping2,

        /// <summary>
        /// Indicates a system sleeping state less than PowerSystemSleeping2 and greater than PowerSystemHibernate, which corresponds to
        /// system power state S3.
        /// </summary>
        PowerSystemSleeping3,

        /// <summary>Indicates the lowest-powered sleeping state, which corresponds to system power state S4.</summary>
        PowerSystemHibernate,

        /// <summary>Indicates the system is turned off, which corresponds to system shutdown state S5.</summary>
        PowerSystemShutdown,

        /// <summary>
        /// The number of system power state values for this enumeration type that represents actual power states. This value is the
        /// number of elements in the DeviceState member of the DEVICE_CAPABILITIES structure for a device. The other system power state
        /// values are less than this value.
        /// </summary>
        PowerSystemMaximum,
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct BATTERY_REPORTING_SCALE
    {
        /// <summary>
        /// The granularity of the capacity reading returned by IOCTL_BATTERY_QUERY_STATUS in milliwatt-hours (mWh). Granularity may
        /// change over time as battery discharge and recharge lowers the range of readings.
        /// </summary>
        public uint Granularity;

        /// <summary>
        /// The upper capacity limit for Granularity. The value of Granularity is valid for capacities reported by
        /// IOCTL_BATTERY_QUERY_STATUS that are less than or equal to this capacity (mWh), but greater than or equal to the capacity
        /// given in the previous array element, or zero if this is the first array element.
        /// </summary>
        public uint Capacity;
    }

    public static class SettingIdLookup
    {
        public static Dictionary<SettingSubgroup, Guid> SettingSubgroupGuids = new Dictionary<SettingSubgroup, Guid>
        {
            { SettingSubgroup.NO_SUBGROUP,                  new Guid("fea3413e-7e05-4911-9a71-700331f1c294") },
            { SettingSubgroup.DISK_SUBGROUP,                new Guid("0012ee47-9041-4b5d-9b77-535fba8b1442") },
            { SettingSubgroup.SYSTEM_BUTTON_SUBGROUP,       new Guid("4f971e89-eebd-4455-a8de-9e59040e7347") },
            { SettingSubgroup.PROCESSOR_SETTINGS_SUBGROUP,  new Guid("54533251-82be-4824-96c1-47b60b740d00") },
            { SettingSubgroup.VIDEO_SUBGROUP,               new Guid("7516b95f-f776-4464-8c53-06167f40cc99") },
            { SettingSubgroup.BATTERY_SUBGROUP,             new Guid("e73a048d-bf27-4f12-9731-8b2076e8891f") },
            { SettingSubgroup.SLEEP_SUBGROUP,               new Guid("238C9FA8-0AAD-41ED-83F4-97BE242C8F20") },
            { SettingSubgroup.PCIEXPRESS_SETTINGS_SUBGROUP, new Guid("501a4d13-42af-4429-9fd1-a8218c268e20") }
        };

        public static Dictionary<Setting, Guid> SettingGuids = new Dictionary<Setting, Guid>
        {
            { Setting.BATACTIONCRIT,    new Guid("637ea02f-bbcb-4015-8e2c-a1c7b9c0b546") },
            { Setting.BATACTIONLOW,     new Guid("d8742dcb-3e6a-4b3c-b3fe-374623cdcf06") },
            { Setting.BATFLAGSLOW,      new Guid("bcded951-187b-4d05-bccc-f7e51960c258") },
            { Setting.BATLEVELCRIT,     new Guid("9a66d8d7-4ff7-4ef9-b5a2-5a326ca2a469") },
            { Setting.BATLEVELLOW,      new Guid("8183ba9a-e910-48da-8769-14ae6dc1170a") },
            { Setting.LIDACTION,        new Guid("5ca83367-6e45-459f-a27b-476b1d01c936") },
            { Setting.PBUTTONACTION,    new Guid("7648efa3-dd9c-4e3e-b566-50f929386280") },
            { Setting.SBUTTONACTION,    new Guid("96996bc0-ad50-47ec-923b-6f41874dd9eb") },
            { Setting.UIBUTTON_ACTION,  new Guid("a7066653-8d6c-40a8-910e-a1f54b84c7e5") },
            { Setting.DISKIDLE,         new Guid("6738e2c4-e8a5-4a42-b16a-e040e769756e") },
            { Setting.ASPM,             new Guid("ee12f906-d277-404b-b6da-e5fa1a576df5") },
            { Setting.PROCFREQMAX,      new Guid("75b0ae3f-bce0-45a7-8c89-c9611c25e100") },
            { Setting.PROCTHROTTLEMAX,  new Guid("bc5038f7-23e0-4960-96da-33abaf5935ec") },
            { Setting.PROCTHROTTLEMIN,  new Guid("893dee8e-2bef-41e0-89c6-b55d0929964c") },
            { Setting.SYSCOOLPOL,       new Guid("94d3a615-a899-4ac5-ae2b-e4d8f634367f") },
            { Setting.HIBERNATEIDLE,    new Guid("9d7815a6-7ee4-497e-8888-515a05f02364") },
            { Setting.HYBRIDSLEEP,      new Guid("94ac6d29-73ce-41a6-809f-6363ba21b47e") },
            { Setting.RTCWAKE,          new Guid("bd3b718a-0680-4d9d-8ab2-e1d2b4ac806d") },
            { Setting.STANDBYIDLE,      new Guid("29f6c1db-86da-48c5-9fdb-f2b67b1f44da") },
            { Setting.ADAPTBRIGHT,      new Guid("fbd9aa66-9553-4097-ba44-ed6e9d65eab8") },
            { Setting.VIDEOIDLE,        new Guid("3c0bc021-c8a8-4e07-a973-6b14cbcb2b7e") }
        };
    }
}
