namespace PowerScheme.Model
{
    using PowerSchemeServiceAPI;
    using System.Windows.Forms;

    public sealed class ViewModel : IViewModel
    {
        public ViewModel(IPowerSchemeService power)
        {
            Power = power;
            ContextLeftMenu = new LeftContextMenu(power);
            ContextRightMenu = new RightContextMenu(power);
        }

        public NotifyIcon NotifyIcon { get; } = new NotifyIcon();

        public ContextMainMenu ContextLeftMenu { get; } 

        public ContextMainMenu ContextRightMenu { get; }

        public void ClearAllMenu()
        {
            ContextLeftMenu.ClearMenu();
            ContextRightMenu.ClearMenu();
        }
        
        public void BuildAllMenu()
        {
            ContextLeftMenu.BuildMenu();
            ContextRightMenu.BuildMenu();
        }

        public IPowerSchemeService Power { get; }

        public void Dispose()
        {
            NotifyIcon?.Dispose();
            ContextLeftMenu?.Dispose();
            ContextRightMenu?.Dispose();
        }
    }
}
