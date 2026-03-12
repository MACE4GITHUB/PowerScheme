using System.Windows.Forms;
using RegistryManager;
using static PowerScheme.MenuLookup;

namespace PowerScheme.Model.Menu.Sleep;

internal class SleepMenuBuilder :
    BaseMenuBuilder
{
    private const MenuItm MENU_ITM = MenuItm.Sleep;

    protected override MenuItm MenuItm => MENU_ITM;

    internal static ToolStripMenuItem Empty => new() { Name = $"{MENU_ITM}" };

    internal ToolStripMenuItem Build()
    {
        Root.Tag = RegistryService.IsShowSleepOption;
        Root.GetCheckedOption();

        var command = new SleepCommand();
        Root.BindCommand(command);

        return Root;
    }
}
