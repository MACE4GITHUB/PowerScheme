using System.Windows.Forms;
using RegistryManager;
using static PowerScheme.MenuLookup;

namespace PowerScheme.Model.Menu.Hibernate;

internal class HibernateMenuBuilder :
    BaseMenuBuilder
{
    private const MenuItm MENU_ITM = MenuItm.Hibernate;

    protected override MenuItm MenuItm => MENU_ITM;

    internal static ToolStripMenuItem Empty => new() { Name = $"{MENU_ITM}" };

    internal ToolStripMenuItem Build()
    {
        Root.Tag = RegistryService.IsShowHibernateOption;
        Root.GetCheckedOption();

        var command = new HibernateCommand();
        Root.BindCommand(command);

        return Root;
    }
}
