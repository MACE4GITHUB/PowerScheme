using System;
using System.Windows.Forms;

namespace PowerScheme.Model
{
    public interface IViewModel: IDisposable
    {
        NotifyIcon NotifyIcon { get; }
        ContextMenuStrip ContextLeftMenu { get; }
        ContextMenuStrip ContextRightMenu { get; }
    }
}