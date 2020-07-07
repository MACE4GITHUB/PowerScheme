using PInvoke;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WinHooks
{
    /// <summary>
    /// Represents native methods.
    /// </summary>
    [ComVisible(false), System.Security.SuppressUnmanagedCodeSecurity()]
    public static class NativeMethods
    {
        #region Private Const
        private const string POWRPROF = "powrprof.dll";
        private const string USER32 = "User32.dll";
        private const string KERNEL32 = "kernel32.dll";
        #endregion

        #region Extern Function
        [DllImport(POWRPROF, SetLastError = true, EntryPoint = "PowerGetActiveScheme")]
        private static extern uint PowerGetActiveScheme(IntPtr UserRootPowerKey, out IntPtr SchemeGuid);

        [DllImport(POWRPROF, SetLastError = true, EntryPoint = "PowerSetActiveScheme")]
        private static extern uint PowerSetActiveScheme(IntPtr UserRootPowerKey, ref Guid SchemeGuid);

        [DllImport(USER32, SetLastError = true, EntryPoint = "RegisterPowerSettingNotification", CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr RegisterPowerSettingNotification(IntPtr hRecipient, ref Guid PowerSettingGuid, Int32 Flags);

        [DllImport(USER32, EntryPoint = "UnregisterPowerSettingNotification", CallingConvention = CallingConvention.StdCall)]
        private static extern bool UnregisterPowerSettingNotification(IntPtr handle);

        [DllImport(USER32, CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport(USER32, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(HookType hookType, WindowsHookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport(KERNEL32, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct POWERBROADCAST_SETTING
        {
            public Guid PowerSetting;
            public uint DataLength;
            public byte Data;
        }

        public enum HookType : int
        {
            WH_JOURNALRECORD = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD = 2,
            WH_GETMESSAGE = 3,
            WH_CALLWNDPROC = 4,
            WH_CBT = 5,
            WH_SYSMSGFILTER = 6,
            WH_MOUSE = 7,
            WH_HARDWARE = 8,
            WH_DEBUG = 9,
            WH_SHELL = 10,
            WH_FOREGROUNDIDLE = 11,
            WH_CALLWNDPROCRET = 12,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }

        #endregion

        public static readonly Guid GUID_POWERSCHEME_PERSONALITY = new Guid("245d8541-3943-4422-b025-13A784F679B7");

        public delegate int WindowsHookProc(int nCode, IntPtr wParam, IntPtr lParam);

        #region ActiveGuid
        public static Guid GetActiveGuid()
        {
            Guid activeSchema;
            var guidPtr = IntPtr.Zero;

            try
            {
                var result = PowerGetActiveScheme(IntPtr.Zero, out guidPtr);

                if (result != 0)
                {
                    throw new Exception($"GetActiveGuid() failed with code {result}");
                }

                if (guidPtr == IntPtr.Zero)
                {
                    throw new Exception("GetActiveGuid() returned null pointer for GUID");
                }

                activeSchema = (Guid)Marshal.PtrToStructure(guidPtr, typeof(Guid));
            }
            finally
            {
                if (guidPtr != IntPtr.Zero)
                {
                    Kernel32.LocalFree(guidPtr);
                }
            }

            return activeSchema;
        }

        public static void SetActiveGuid(Guid guid)
        {
            var result = PowerSetActiveScheme(IntPtr.Zero, ref guid);
            if (result != 0)
            {
                throw new Exception($"SetActiveGuid() failed with code {result}");
            }
        }
        #endregion
        
        public static void HideWindowFromAltTab(IntPtr windowHandle)
        {
            int exStyle = User32.GetWindowLong(windowHandle, User32.WindowLongIndexFlags.GWL_EXSTYLE);
            exStyle |= (int)User32.WindowStylesEx.WS_EX_TOOLWINDOW;

            User32.SetWindowLong(windowHandle, User32.WindowLongIndexFlags.GWL_EXSTYLE, (User32.SetWindowLongFlags)exStyle);
        }
    }
}
