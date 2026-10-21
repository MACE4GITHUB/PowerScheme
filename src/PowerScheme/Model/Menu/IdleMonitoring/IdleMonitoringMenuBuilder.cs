using System;
using System.Linq;
using System.Windows.Forms;
using PowerScheme.Utility;
using PowerSchemeServiceAPI;
using RegistryManager;
using static PowerScheme.MenuLookup;

namespace PowerScheme.Model.Menu.IdleMonitoring;

internal class IdleMonitoringMenuBuilder(
    IPowerSchemeService power,
    ToolStripMenuItem? root = null) :
    BaseMenuBuilder(root)
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

        return Complete();
    }

    private void AddIdleMonitoringItems()
    {
        AddIdleDoNothing();

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
            var item = Menu.Item(powerSchemeMenuItem.Key);
            item.Tag = powerSchemeMenuItem.Value.Tag;

            item.BindCommandOnce(static () => new IdleMonitoringCommand());
        }

        var wellKnownSchemeMenuItemIds = wellKnownSchemeMenuItems
            .Select(mi => (Guid)mi.Value.Tag!)
            .ToList();

        var unknownSchemeMenuItems = powerSchemes.Where(x => !wellKnownSchemeMenuItemIds.Contains(x.Guid));

        foreach (var powerScheme in unknownSchemeMenuItems)
        {
            var item = Menu.Item($"scheme_{powerScheme.Guid}", powerScheme.Name, powerScheme.Picture.GetMenuImage());
            item.Tag = powerScheme.Guid;

            item.BindCommandOnce(static () => new IdleMonitoringCommand());
        }

        var prev = Root.Image;
        Root.Image = lidPowerScheme != null
            ? lidPowerScheme.Picture.GetMenuImage()
            : MenuItems[MenuItm.IdleDoNothing].Picture.GetMenuImage();
        ImageExtensions.DisposeOwnedImage(prev);
    }

    private void AddIdleDoNothing()
    {
        var doNothingItem = Menu.Item(MenuItm.IdleDoNothing);
        doNothingItem.Tag = Guid.Empty;

        doNothingItem.BindCommandOnce(static () => new IdleMonitoringCommand());
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

        Menu.Separator();

        var keepBrightness = Menu.Item(MenuItm.KeepBrightness);
        keepBrightness.Tag = RegistryService.GetKeepBrightness(AppInfo.CompanyName, AppInfo.ProductName);
        keepBrightness.GetCheckedOption();

        keepBrightness.BindCommandOnce(static () => new KeepBrightnessCommand());
    }

    private void AddIdleForm()
    {
        Menu.Separator();

        var showIdleOptions = Menu.Item(MenuItm.ShowIdleOptions);

        showIdleOptions.BindCommandOnce(static () => new ShowIdleFormCommand());
    }

    private void AddIdleDisplayForm()
    {
        var showIdleDisplayOptions = Menu.Item(MenuItm.ShowIdleDisplayOptions);

        showIdleDisplayOptions.BindCommandOnce(static () => new ShowIdleDisplayFormCommand());
    }

    private void AddIdleSleepForm()
    {
        var showIdleSleepOptions = Menu.Item(MenuItm.ShowIdleSleepOptions);

        showIdleSleepOptions.BindCommandOnce(static () => new ShowIdleSleepFormCommand());
    }
}
