using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using PowerSchemeServiceAPI;

namespace PowerScheme.Model;

public sealed class ViewModel : IViewModel
{
    public ViewModel(
        IContainer container,
        LeftContextMenu leftContextMenu,
        RightContextMenu rightContextMenu,
        IPowerSchemeService power)
    {
        NotifyIcon = new NotifyIcon(container);
        ContextLeftMenu = leftContextMenu;
        ContextRightMenu = rightContextMenu;
        Power = power;
    }

    public NotifyIcon NotifyIcon { get; }

    public ContextMainMenu ContextLeftMenu { get; }

    public ContextMainMenu ContextRightMenu { get; }

    public IPowerSchemeService Power { get; }

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

    public void UpdateIcon(Icon icon)
    {
        RemoveIcon();
        NotifyIcon.Icon = icon;
    }

    public void RemoveIcon()
    {
        var currentIcon = NotifyIcon.Icon;

        if (currentIcon is null)
        {
            return;
        }

        // Remove reference from NotifyIcon first, then dispose the icon.
        NotifyIcon.Icon = null;
        currentIcon.Dispose();
    }

    public void Dispose()
    {
        RemoveIcon();
    }
}
