using System;
using System.Linq;
using System.Windows.Forms;
using PowerScheme.Utility;
using PowerSchemeServiceAPI;
using RegistryManager;
using static PowerScheme.MenuLookup;

namespace PowerScheme.Model.Menu.IdleMonitoring;

internal class IdleMonitoringMenuBuilder(
    IPowerSchemeService power) :
    BaseMenuBuilder
{
    private const MenuItm MENU_ITM = MenuItm.IdleMonitoring;

    protected override MenuItm MenuItm => MENU_ITM;

    internal static ToolStripMenuItem Empty => new() { Name = $"{MENU_ITM}" };

    internal ToolStripMenuItem Build()
    {
        AddIdleMonitoringItems();

        return Root;
    }

    private void AddIdleMonitoringItems()
    {
        var dropDownItems = Root.DropDownItems;

        AddIdleDoNothing(dropDownItems);

        var activePowerSchemeId = power.ActivePowerScheme.Guid;

        var lidPowerSchemeId = RegistryService.GetIdleMonitoring(AppInfo.CompanyName, AppInfo.ProductName);
        var lidPowerScheme = power.PowerSchemes
            .FirstOrDefault(x => !x.IsMaxPerformance && x.Guid == lidPowerSchemeId);

        var powerSchemes = power.PowerSchemes
            .Where(x => !x.IsMaxPerformance && x.Guid != activePowerSchemeId).ToList();

        var powerSchemeGuids = powerSchemes
            .Select(x => x.Guid).ToList();

        var wellKnownSchemeMenuItems = MenuItems
            .Where(mi => mi.Value.MenuItmKind == MenuItmKind.PowerScheme &&
                         mi.Value.Tag is Guid g &&
                         g != activePowerSchemeId &&
                         powerSchemeGuids.Contains(g))
            .ToList();

        foreach (var powerSchemeMenuItem in wellKnownSchemeMenuItems)
        {
            var item = MenuItemFactory.Create(powerSchemeMenuItem.Key);
            item.Tag = powerSchemeMenuItem.Value.Tag;

            var command = new IdleMonitoringCommand();
            item.BindCommand(command);

            dropDownItems.Add(item);
        }

        var wellKnownSchemeMenuItemIds = wellKnownSchemeMenuItems
            .Select(mi => (Guid)mi.Value.Tag!)
            .ToList();

        var unknownSchemeMenuItems = powerSchemes.Where(x => !wellKnownSchemeMenuItemIds.Contains(x.Guid));

        foreach (var powerScheme in unknownSchemeMenuItems)
        {
            var item = new ToolStripMenuItem
            {
                Tag = powerScheme.Guid,
                Text = powerScheme.Name,
                Image = powerScheme.Picture.GetImage()
            };

            var command = new IdleMonitoringCommand();
            item.BindCommand(command);

            dropDownItems.Add(item);
        }

        var prev = Root.Image;
        Root.Image = lidPowerScheme != null
            ? lidPowerScheme.Picture.GetImage()
            : MenuItems[MenuItm.IdleDoNothing].Picture.GetImage();
        prev?.Dispose();
    }

    private static void AddIdleDoNothing(ToolStripItemCollection dropDownItems)
    {
        var doNothingItem = MenuItemFactory.Create(MenuItm.IdleDoNothing);
        doNothingItem.Tag = Guid.Empty;

        var command = new IdleMonitoringCommand();
        doNothingItem.BindCommand(command);

        dropDownItems.Add(doNothingItem);
    }
}
