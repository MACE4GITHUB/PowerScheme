namespace MessageForm
{
    using System;
    using System.Runtime.InteropServices;

    internal static class NativeMethods
    {
        private const string USER32 = "user32.dll";

        [DllImport(USER32, SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "FindWindow")]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport(USER32, CharSet = CharSet.Auto, EntryPoint = "SendMessage")]
        internal static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);

        [DllImport(USER32, CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetActiveWindow();
    }
}
