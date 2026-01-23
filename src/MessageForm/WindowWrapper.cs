using System;
using System.Windows.Forms;

namespace MessageForm;

/// <summary>
/// Provides an implementation of the IWin32Window interface for wrapping a native window handle.
/// </summary>
/// <param name="handle">The handle to the native window to be wrapped.</param>
internal class WindowWrapper(IntPtr handle) : IWin32Window
{
    public IntPtr Handle { get; } = handle;
}
