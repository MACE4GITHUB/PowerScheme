using System.Windows.Forms;
using RegistryManager;
using static PowerScheme.MenuLookup;

namespace PowerScheme.Model.Menu.WindowsStartup;

internal class WindowsStartupMenuBuilder :
    BaseMenuBuilder
{
    private const MenuItm MENU_ITM = MenuItm.StartupOnWindows;

    protected override MenuItm MenuItm => MENU_ITM;

    internal static ToolStripMenuItem Empty => new() { Name = $"{MENU_ITM}" };

    internal ToolStripMenuItem Build()
    {
        Root.Tag = RegistryService.IsRunOnStartup;
        Root.GetCheckedOption();

        var command = new WindowsStartupCommand();
        Root.BindCommand(command);

        return Root;
    }
}
