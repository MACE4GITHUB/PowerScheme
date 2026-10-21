using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PowerScheme.Utility;
using PowerSchemeServiceAPI;
using PowerSchemeServiceAPI.Model;

namespace PowerScheme.Model.Menu.PowerSchemes;

using static MenuLookup;

internal class PowerSchemeMenuBuilder(
    IPowerSchemeService power) :
    BaseMenuBuilder
{
    private const MenuItm MENU_ITM = MenuItm.PowerSchemes;

    protected override MenuItm MenuItm => MENU_ITM;

    internal static ToolStripMenuItem Empty => new() { Name = $"{MENU_ITM}" };

    internal ToolStripMenuItem Build()
    {
        try
        {
            Update(Root.DropDownItems, power);
            return Root;
        }
        catch
        {
            Root.SafeRemove();
            throw;
        }
    }

    internal static void Update(ToolStripItemCollection items, IPowerSchemeService power)
    {
        var update = new MenuItemUpdater(items);
        AddPowerSchemes(update, power, power.PowerProfPowerSchemes);
        if (power.UserPowerSchemes.Any())
        {
            update.Separator();
            AddPowerSchemes(update, power, power.UserPowerSchemes);
        }
        update.Complete();
    }

    private static void AddPowerSchemes(
        MenuItemUpdater update,
        IPowerSchemeService power,
        IEnumerable<IPowerScheme> powerSchemes)
    {
        foreach (var powerScheme in powerSchemes)
        {
            var item = update.Item($"scheme_{powerScheme.Guid}", powerScheme.Name, powerScheme.Picture.GetMenuImage());
            item.Tag = new StatePowerScheme(powerScheme);

            item.BindCommandOnce(power, static service => new PowerSchemeCommand(service));
        }
    }
}
