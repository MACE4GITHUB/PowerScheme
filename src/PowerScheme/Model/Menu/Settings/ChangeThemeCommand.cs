using System;
using System.Windows.Forms;
using PowerScheme.Model.Command;
using PowerScheme.Themes;
using RegistryManager;

namespace PowerScheme.Model.Menu.Settings;

public class ChangeThemeCommand :
    IMenuEventHandlerCommand
{
    public void Execute(object sender, EventArgs e)
    {
        if (sender is ToolStripMenuItem { Tag: ThemeKind tag })
        {
            RegistryService.SetTheme(AppInfo.CompanyName, AppInfo.ProductName, (int)tag);
        }
    }
}
