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
        AddPowerSchemes(power.PowerProfPowerSchemes);

        if (!power.UserPowerSchemes.Any())
        {
            return Root;
        }

        Root.DropDownItems.Add(new ToolStripSeparator());

        AddPowerSchemes(power.UserPowerSchemes);

        return Root;
    }

    private void AddPowerSchemes(
        IEnumerable<IPowerScheme> powerSchemes)
    {
        var dropDownItems = Root.DropDownItems;

        foreach (var powerScheme in powerSchemes)
        {
            var item = new ToolStripMenuItem
            {
                Name = $"{powerScheme.Name}_ps",
                Text = powerScheme.Name,
                Tag = new StatePowerScheme(powerScheme),
                Image = powerScheme.Picture.GetImage()
            };

            var command = new PowerSchemeCommand(power);
            item.BindCommand(command);

            dropDownItems.Add(item);
        }
    }
}
