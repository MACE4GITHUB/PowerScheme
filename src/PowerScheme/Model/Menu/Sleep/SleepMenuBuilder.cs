using System.Windows.Forms;
using RegistryManager;
using static PowerScheme.MenuLookup;

namespace PowerScheme.Model.Menu.Sleep;

internal class SleepMenuBuilder(ToolStripMenuItem? root = null) :
    BaseMenuBuilder(root)
{
    private const MenuItm MENU_ITM = MenuItm.Sleep;

    protected override MenuItm MenuItm => MENU_ITM;

    internal static ToolStripMenuItem Empty => new() { Name = $"{MENU_ITM}" };

    internal ToolStripMenuItem Build()
    {
        Root.Tag = RegistryService.IsShowSleepOption;
        Root.GetCheckedOption();

        Root.BindCommandOnce(static () => new SleepCommand());

        return Complete();
    }
}
