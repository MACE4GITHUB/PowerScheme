using RegistryManager.Model;
using System;
using System.Runtime.InteropServices;

namespace RegistryManager
{
    [ComVisible(false), System.Security.SuppressUnmanagedCodeSecurity()]
    internal static class NativeMethods
    {
        private const string ADVAPI32 = "advapi32.dll";

        [DllImport(ADVAPI32, SetLastError = true)]
        public static extern int RegOpenKeyEx(IntPtr hKey, string subKey, uint options, int samDesired,
            out IntPtr phkResult);

        [DllImport(ADVAPI32, SetLastError = true)]
        public static extern int RegNotifyChangeKeyValue(IntPtr hKey, bool bWatchSubtree,
            RegChangeNotifyFilter dwNotifyFilter, IntPtr hEvent,
            bool fAsynchronous);

        [DllImport(ADVAPI32, SetLastError = true)]
        public static extern int RegCloseKey(IntPtr hKey);
    }
}
