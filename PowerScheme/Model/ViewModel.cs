using System.Windows.Forms;

namespace PowerScheme.Model
{
    public class ViewModel : IViewModel
    {
        public NotifyIcon NotifyIcon { get; } = new NotifyIcon();

        public ContextMenuStrip ContextLeftMenu { get; } = new ContextMenuStrip();

        public ContextMenuStrip ContextRightMenu { get; } = new ContextMenuStrip();
    }
}
