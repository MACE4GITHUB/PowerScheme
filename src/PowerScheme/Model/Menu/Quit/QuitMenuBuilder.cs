using System.Windows.Forms;
using static PowerScheme.MenuLookup;

namespace PowerScheme.Model.Menu.Quit;

internal class QuitMenuBuilder :
    BaseMenuBuilder
{
    private const MenuItm MENU_ITM = MenuItm.Quit;

    protected override MenuItm MenuItm => MENU_ITM;

    internal static ToolStripMenuItem Empty => new() { Name = $"{MENU_ITM}" };

    internal ToolStripMenuItem Build()
    {
        var command = new QuitCommand();
        Root.BindCommand(command);

        return Root;
    }
}
