using System.Windows.Forms;
using Languages;
using PowerScheme.Services;
using static PowerScheme.MenuLookup;

namespace PowerScheme.Model.Menu.Version;

internal class VersionMenuBuilder(
    IUpdateService updateService) :
    BaseMenuBuilder
{
    private const MenuItm MENU_ITM = MenuItm.Version;

    protected override MenuItm MenuItm => MENU_ITM;

    internal static ToolStripMenuItem Empty => new() { Name = $"{MENU_ITM}" };

    internal ToolStripMenuItem Build()
    {
        var root = new ToolStripMenuItem
        {
            Name = nameof(MenuItm.Version),
            Text = $@"{AppInfo.ProductName} {AppInfo.ProductVersion}"
        };

        if (updateService.ReleaseInfo.NewVersionAvailable)
        {
            AddUpdateAppMenu(root);
        }

        return root;
    }

    private void AddUpdateAppMenu(ToolStripMenuItem root)
    {
        var infoMenuItem = MenuItemFactory.Create(MenuItm.NewVersionIsAvailable);
        infoMenuItem.Enabled = false;

        root.DropDownItems.Add(infoMenuItem);

        var releaseInfo = updateService.ReleaseInfo;
        var text = $@"{Language.Current.UpdateAppToVersion} {releaseInfo.RemoteVersion}";

        var updateMenuItem = releaseInfo.NewVersionAvailable
            ? MenuItemFactory.Create(MenuItm.UpdateApp, text)
            : MenuItemFactory.Create(MenuItm.UpdateApp);

        var command = new UpdateAppCommand(updateService);
        updateMenuItem.BindCommand(command);

        root.DropDownItems.Add(updateMenuItem);
    }
}
