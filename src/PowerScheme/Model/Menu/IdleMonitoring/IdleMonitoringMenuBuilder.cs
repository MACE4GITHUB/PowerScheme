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
        AddKeepingBrightness();
        AddIdleForm();
        AddIdleDisplayForm();
        AddIdleSleepForm();

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

    private void AddKeepingBrightness()
    {
        if (!power.ExistsMobilePlatformRole)
        {
            return;
        }

        var lidPowerSchemeId = RegistryService.GetIdleMonitoring(AppInfo.CompanyName, AppInfo.ProductName);

        if (lidPowerSchemeId == Guid.Empty)
        {
            return;
        }

        var dropDownItems = Root.DropDownItems;

        dropDownItems.Add(new ToolStripSeparator());

        var keepBrightness = MenuItemFactory.Create(MenuItm.KeepBrightness);
        keepBrightness.Tag = RegistryService.GetKeepBrightness(AppInfo.CompanyName, AppInfo.ProductName);
        keepBrightness.GetCheckedOption();

        var command = new KeepBrightnessCommand();
        keepBrightness.BindCommand(command);

        dropDownItems.Add(keepBrightness);
    }

    private void AddIdleForm()
    {
        var dropDownItems = Root.DropDownItems;

        dropDownItems.Add(new ToolStripSeparator());

        var showIdleOptions = MenuItemFactory.Create(MenuItm.ShowIdleOptions);

        var command = new ShowIdleFormCommand();
        showIdleOptions.BindCommand(command);

        dropDownItems.Add(showIdleOptions);
    }

    private void AddIdleDisplayForm()
    {
        var dropDownItems = Root.DropDownItems;

        var showIdleDisplayOptions = MenuItemFactory.Create(MenuItm.ShowIdleDisplayOptions);

        var command = new ShowIdleDisplayFormCommand();
        showIdleDisplayOptions.BindCommand(command);

        dropDownItems.Add(showIdleDisplayOptions);
    }

    private void AddIdleSleepForm()
    {
        var dropDownItems = Root.DropDownItems;

        var showIdleSleepOptions = MenuItemFactory.Create(MenuItm.ShowIdleSleepOptions);

        var command = new ShowIdleSleepFormCommand();
        showIdleSleepOptions.BindCommand(command);

        dropDownItems.Add(showIdleSleepOptions);
    }
}
