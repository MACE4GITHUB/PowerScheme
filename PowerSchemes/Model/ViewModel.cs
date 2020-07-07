using System.Windows.Forms;

namespace PowerSchemes.Model
{
    public class ViewModel
    {
        public ViewModel(NotifyIcon notifyIcon, ContextMenuStrip contextLeftMenu, ContextMenuStrip contextRightMenu)
        {
            NotifyIcon = notifyIcon;
            ContextLeftMenu = contextLeftMenu;
            ContextRightMenu = contextRightMenu;
        }

        public NotifyIcon NotifyIcon { get; }

        public ContextMenuStrip ContextLeftMenu { get; }

        public ContextMenuStrip ContextRightMenu { get; }
    }
}
