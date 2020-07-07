namespace FormAutoClose
{
    using System;
    using static System.Runtime.InteropServices.CharSet;

    internal static class NativeMethods
    {
        private const string USER32 = "user32.dll";

        [System.Runtime.InteropServices.DllImport(USER32, SetLastError = true, CharSet = Unicode, EntryPoint = "FindWindow")]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [System.Runtime.InteropServices.DllImport(USER32, CharSet = Auto, EntryPoint = "SendMessage")]
        internal static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);
    }
}
