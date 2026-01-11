using System;
using System.Windows.Forms;

namespace MessageForm;

internal class WindowWrapper(IntPtr handle) : IWin32Window
{
    public IntPtr Handle { get; } = handle;
}
