using System;
using System.Linq;
using System.Windows.Forms;
using Languages;
using PowerScheme.Utility;
using PowerSchemeServiceAPI;
using PowerSchemeServiceAPI.Model;
using static PowerScheme.MenuLookup;

namespace PowerScheme.Model.Menu.Settings;

internal class SettingsMenuBuilder :
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
        AddCustomPowerSchemes(root);

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
            item.Text = _power.TextActionToggle(tag);
            item.Tag = _power.StatePowerSchemeToggle(tag);

            var command = new ActionPowerSchemeCommand(_power);
            item.BindCommand(command);

            root.DropDownItems.Add(item);
        }
    }

    private void AddCustomPowerSchemes(ToolStripMenuItem root)
    {
        var customPowerSchemes = _power.CustomPowerSchemes.ToList();

        if (customPowerSchemes.Count == 0)
        {
            return;
        }

        root.DropDownItems.Add(new ToolStripSeparator());


        foreach (var scheme in customPowerSchemes)
        {
            var item = new ToolStripMenuItem
            {
                Name = $"scheme_{scheme.Guid}",
                Text = $@"{Language.Current.DeleteCustomScheme} '{scheme.Name}'",
                Image = scheme.Picture.GetImage(),
                ImageScaling = ToolStripItemImageScaling.SizeToFit,
            };

            var tag = new StatePowerScheme(scheme);
            item.Tag = tag;

            var command = new DeletePowerSchemeCommand(_power);
            item.BindCommand(command);

            root.DropDownItems.Add(item);
        }
    }
}
