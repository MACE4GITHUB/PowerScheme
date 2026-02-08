using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using PowerSchemeServiceAPI;

namespace PowerScheme.Model;

public sealed class ViewModel(
    IContainer container,
    LeftContextMenu leftContextMenu,
    RightContextMenu rightContextMenu,
    IPowerSchemeService power) : IViewModel
{
    public NotifyIcon NotifyIcon { get; } = new NotifyIcon(container);

    public ContextMainMenu ContextLeftMenu { get; } = leftContextMenu;

    public ContextMainMenu ContextRightMenu { get; } = rightContextMenu;

    public IPowerSchemeService Power { get; } = power;

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
