using System;
using System.Linq;
using System.Windows.Forms;
using PowerSchemeServiceAPI;
using PowerSchemeServiceAPI.Model;
using static PowerScheme.MenuLookup;

namespace PowerScheme.Model.Menu.Settings;

internal class SettingsMenuBuilder:
    BaseMenuBuilder
{
    private readonly IPowerSchemeService _power;

    private const MenuItm MENU_ITM = MenuItm.Settings;

    protected override MenuItm MenuItm => MENU_ITM;

    internal static ToolStripMenuItem Empty => new() { Name = $"{MENU_ITM}" };

    internal SettingsMenuBuilder(IPowerSchemeService power) =>
        _power = power ?? throw new ArgumentNullException(nameof(power));

    internal ToolStripMenuItem Build()
    {
        var root = new ToolStripMenuItem
        {
            Name = $"{MenuItm}",
            Text = MenuItems[MenuItm].Name
        };

        AddControlPanelScheme(root);
        AddRestoreDefault(root);
        root.DropDownItems.Add(new ToolStripSeparator());

        var withoutDeletedCount = _power.TypicalPowerSchemesWithoutDeleted.Count();
        var withDeletedCount = _power.TypicalPowerSchemesWithDeleted.Count();
        var onlyDeletedCount = withDeletedCount - withoutDeletedCount;

        if (onlyDeletedCount >= 0 && withoutDeletedCount != 0)
        {
            AddDeleteTypicalSchemes(root);
        }

        if (onlyDeletedCount > 0)
        {
            AddTypicalSchemes(root);
        }

        root.DropDownItems.Add(new ToolStripSeparator());

        AddDynamicPowerSchemes(root);

        return root;
    }

    private static void AddControlPanelScheme(ToolStripMenuItem root)
    {
        var item = MenuItemFactory.Create(
            MenuItm.ControlPanelSchemeWindows);

        var command = new OpenPowerSchemesPanelCommand();
        item.BindCommand(command);

        root.DropDownItems.Add(item);
    }

    private void AddRestoreDefault(ToolStripMenuItem root)
    {
        var item = MenuItemFactory.Create(
            MenuItm.RestoreDefaultPowerSchemes);

        var command = new RestoreDefaultPowerSchemesCommand(_power);
        item.BindCommand(command);

        root.DropDownItems.Add(item);
    }

    private void AddTypicalSchemes(ToolStripMenuItem root)
    {
        var item = MenuItemFactory.Create(
            MenuItm.CreateTypicalSchemes);

        var command = new CreateTypicalPowerSchemesCommand(_power);
        item.BindCommand(command);

        root.DropDownItems.Add(item);
    }

    private void AddDeleteTypicalSchemes(ToolStripMenuItem root)
    {
        var item = MenuItemFactory.Create(MenuItm.DeleteTypicalSchemes);

        var command = new DeleteTypicalPowerSchemesCommand(_power);
        item.BindCommand(command);

        root.DropDownItems.Add(item);
    }

    private void AddDynamicPowerSchemes(ToolStripMenuItem root)
    {
        foreach (var scheme in _power.TypicalPowerSchemesWithDeleted)
        {
            var menuItem = MenuItems
                .First(x => x.Value.Picture == scheme.Picture)
                .Key;

            var item = MenuItemFactory.Create(menuItem);

            var tag = new StatePowerScheme(scheme, ActionWithPowerScheme.Create);
            item.Tag = tag;
            item.Text = _power.TextActionToggle(tag);
            item.Tag = _power.StatePowerSchemeToggle(tag);

            var command = new ActionPowerSchemeCommand(_power);
            item.BindCommand(command);

            root.DropDownItems.Add(item);
        }
    }
}
