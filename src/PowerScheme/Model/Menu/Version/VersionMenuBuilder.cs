using System.Windows.Forms;
using Languages;
using PowerScheme.Services;
using static PowerScheme.MenuLookup;

namespace PowerScheme.Model.Menu.Version;

internal class VersionMenuBuilder(
    IUpdateService updateService,
    ToolStripMenuItem? root = null) :
    BaseMenuBuilder(root)
{
    private const MenuItm MENU_ITM = MenuItm.Version;

    protected override MenuItm MenuItm => MENU_ITM;

    internal static ToolStripMenuItem Empty => new() { Name = $"{MENU_ITM}" };

    internal ToolStripMenuItem Build()
    {
        Root.Text = $@"{AppInfo.ProductName} {AppInfo.ProductVersion}";
        Root.Image = null;
        Root.ToolTipText = string.Empty;

        if (updateService.ReleaseInfo.NewVersionAvailable)
        {
            AddUpdateAppMenu();
        }

        return Complete();
    }

    private void AddUpdateAppMenu()
    {
        var infoMenuItem = Menu.Item(MenuItm.NewVersionIsAvailable);
        infoMenuItem.Enabled = false;

        var releaseInfo = updateService.ReleaseInfo;
        var text = $@"{Language.Current.UpdateAppToVersion} {releaseInfo.RemoteVersion}";

        var updateMenuItem = releaseInfo.NewVersionAvailable
            ? Menu.Item(MenuItm.UpdateApp, text)
            : Menu.Item(MenuItm.UpdateApp);

        updateMenuItem.BindCommandOnce(updateService, static service => new UpdateAppCommand(service));

    }
}
