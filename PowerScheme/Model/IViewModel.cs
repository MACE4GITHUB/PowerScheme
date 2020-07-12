using System.Windows.Forms;

namespace PowerScheme.Model
{
    public interface IViewModel
    {
        NotifyIcon NotifyIcon { get; }
        ContextMenuStrip ContextLeftMenu { get; }
        ContextMenuStrip ContextRightMenu { get; }
    }
}