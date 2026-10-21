using System.Windows.Forms;
using static PowerScheme.MenuLookup;

namespace PowerScheme.Model.Menu.Quit;

internal class QuitMenuBuilder(ToolStripMenuItem? root = null) :
    BaseMenuBuilder(root)
{
    private const MenuItm MENU_ITM = MenuItm.Quit;

    protected override MenuItm MenuItm => MENU_ITM;

    internal static ToolStripMenuItem Empty => new() { Name = $"{MENU_ITM}" };

    internal ToolStripMenuItem Build()
    {
        Root.BindCommandOnce(static () => new QuitCommand());

        return Complete();
    }
}
