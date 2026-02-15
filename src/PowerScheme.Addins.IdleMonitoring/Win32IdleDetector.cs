using System;
using System.Runtime.InteropServices;

namespace PowerScheme.Addins.IdleMonitoring;

public class Win32IdleDetector : IIdleDetector
{
    private const int DEFAULT_IDLE_TIMEOUT_IN_SECONDS = 5;

    public TimeSpan GetIdleTime()
    {
        var info = new LASTINPUTINFO();
        info.cbSize = (uint)Marshal.SizeOf(info);

        if (!GetLastInputInfo(ref info))
        {
            return TimeSpan.FromSeconds(DEFAULT_IDLE_TIMEOUT_IN_SECONDS);
        }

        var idleTicks = unchecked((uint)Environment.TickCount - info.dwTime);
        return TimeSpan.FromMilliseconds(idleTicks);
    }

    #region DLL Imports

    [StructLayout(LayoutKind.Sequential)]
    private struct LASTINPUTINFO
    {
        public uint cbSize;
        public uint dwTime;
    }

    [DllImport("user32.dll")]
    private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

    #endregion
}
