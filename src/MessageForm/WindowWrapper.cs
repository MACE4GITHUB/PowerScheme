namespace MessageForm;

using System;
using System.Windows.Forms;

internal class WindowWrapper : IWin32Window
{
    public WindowWrapper(IntPtr handle)
    {
        Handle = handle;
    }

    public IntPtr Handle { get; }
}