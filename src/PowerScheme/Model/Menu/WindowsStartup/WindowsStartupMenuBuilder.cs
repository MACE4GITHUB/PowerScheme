using System.Windows.Forms;
using RegistryManager;
using static PowerScheme.MenuLookup;

namespace PowerScheme.Model.Menu.WindowsStartup;

internal class WindowsStartupMenuBuilder(ToolStripMenuItem? root = null) :
    BaseMenuBuilder(root)
{
    private const MenuItm MENU_ITM = MenuItm.StartupOnWindows;

    protected override MenuItm MenuItm => MENU_ITM;

    internal static ToolStripMenuItem Empty => new() { Name = $"{MENU_ITM}" };

    internal ToolStripMenuItem Build()
    {
        Root.Tag = RegistryService.IsRunOnStartup;
        Root.GetCheckedOption();

        Root.BindCommandOnce(static () => new WindowsStartupCommand());

        return Complete();
    }
}
