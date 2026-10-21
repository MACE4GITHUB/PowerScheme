using System.Windows.Forms;
using static PowerScheme.MenuLookup;

namespace PowerScheme.Model.Menu;

internal abstract class BaseMenuBuilder
{
    protected BaseMenuBuilder(ToolStripMenuItem? root = null)
    {
        Root = root ?? MenuItemFactory.Create(MenuItm);
        if (root != null)
        {
            MenuItemFactory.Update(Root, MenuItm);
        }
        Menu = new MenuItemUpdater(Root.DropDownItems);
    }

    protected abstract MenuItm MenuItm { get; }

    protected ToolStripMenuItem Root { get; }

    protected MenuItemUpdater Menu { get; }

    protected ToolStripMenuItem Complete()
    {
        Menu.Complete();
        return Root;
    }
}
