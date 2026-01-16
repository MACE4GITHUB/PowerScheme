using System.ComponentModel;
using System.Windows.Forms;
using PowerSchemeServiceAPI;

namespace PowerScheme.Model;

public sealed class ViewModel : IViewModel
{
    private readonly Container _components;

    public ViewModel(IPowerSchemeService power)
    {
        _components = new Container();
        Power = power;
        ContextLeftMenu = new LeftContextMenu(_components, power);
        ContextRightMenu = new RightContextMenu(_components, power);
        NotifyIcon = new NotifyIcon(_components);
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
        _components?.Dispose();
    }
}