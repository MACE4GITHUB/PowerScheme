using System.ComponentModel;

namespace PowerScheme.Model;

using PowerSchemeServiceAPI;
using System.Windows.Forms;

public sealed class ViewModel : IViewModel
{
    private readonly IContainer components;

    public ViewModel(IPowerSchemeService power)
    {
        components = new Container();
        Power = power;
        ContextLeftMenu = new LeftContextMenu(components, power);
        ContextRightMenu = new RightContextMenu(components, power);
        NotifyIcon = new NotifyIcon(components);
    }

    public NotifyIcon NotifyIcon { get; }

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
        components?.Dispose();
    }
}