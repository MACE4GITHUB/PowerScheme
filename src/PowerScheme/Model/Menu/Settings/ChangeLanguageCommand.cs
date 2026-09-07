using System;
using System.Windows.Forms;
using Languages;
using PowerScheme.Model.Command;
using PowerSchemeServiceAPI;
using RegistryManager;

namespace PowerScheme.Model.Menu.Settings;

public class ChangeLanguageCommand(
    IPowerSchemeService power) :
    IMenuEventHandlerCommand
{
    public void Execute(object sender, EventArgs e)
    {
        if (sender is ToolStripMenuItem { Tag: LanguageKind tag })
        {
            RegistryService.SetLanguage(AppInfo.CompanyName, AppInfo.ProductName, (int)tag);
            Language.SetLanguage(tag);
            power.RenameTypicalSchemes();
            MenuLookup.RebuildMenuItems();
        }
    }
}
