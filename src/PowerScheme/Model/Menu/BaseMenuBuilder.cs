using System.Windows.Forms;
using static PowerScheme.MenuLookup;

namespace PowerScheme.Model.Menu;

internal abstract class BaseMenuBuilder
{
    protected BaseMenuBuilder() =>
        Root = MenuItemFactory.Create(MenuItm);

    protected abstract MenuItm MenuItm { get; }

    protected ToolStripMenuItem Root { get; }
}
