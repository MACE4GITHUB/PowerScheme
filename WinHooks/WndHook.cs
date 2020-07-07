using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using PInvoke;
using static WinHooks.NativeMethods;

namespace WinHooks
{
    public class WndHook : BaseWndHook
    {
        private readonly WindowsHookProc _hookProc;
        private IntPtr _hookId = IntPtr.Zero;

        public WndHook()
        {
            //var guid = GUID_POWERSCHEME_PERSONALITY;
            //RegisterPowerSettingNotification(windowHandle, ref guid, 0);

            const int THREAD = 0;
            _hookProc = WndProcHook;
            //_hookId = SetWindowsHookEx(HookType.WH_CALLWNDPROC, _hookProc, windowHandle, thread);

            using (var curProcess = Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                if (curModule == null) return;
                var hModule = GetModuleHandle(curModule.ModuleName);
                _hookId = SetWindowsHookEx(HookType.WH_CALLWNDPROC, _hookProc, hModule, THREAD);
                //if (_hookId == IntPtr.Zero)
                //    throw new ArgumentException("Invalid parameter: " +
                //                                nameof(_hookId) + " is " +
                //                                _hookId.ToString() +
                //                                ". LastWin32Error = " +
                //                                Marshal.GetLastWin32Error());
            }
        }

        private int WndProcHook(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (0 > nCode)
                return User32.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);

            PowerSchemeChanged?.Invoke(null, EventArgs.Empty);

            // We're only interested in WM_POWERBROADCAST (0x0218) messages with wParam of PBT_POWERSETTINGCHANGE (0x8013)
            if ((int)wParam == 0x8013)
            {
                var settingMessage =
                    (POWERBROADCAST_SETTING)Marshal.PtrToStructure(
                        lParam,
                        typeof(POWERBROADCAST_SETTING));

                // We're only interested if the PowerSetting changed is GUID_POWERSCHEME_PERSONALITY
                if (settingMessage.PowerSetting == GUID_POWERSCHEME_PERSONALITY && settingMessage.DataLength == Marshal.SizeOf(typeof(Guid)))
                {
                    try
                    {
                        PowerSchemeChanged?.Invoke(null, EventArgs.Empty);
                    }
                    catch
                    {
                        // TODO: log?  Or is that too expensive here?
                    }
                }
            }

            return User32.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        public event EventHandler PowerSchemeChanged;

        protected override void UnHookWindowsHook()
        {
            UnhookWindowsHookEx(_hookId);
        }
    }
}
