using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Languages;
using PowerScheme.Configuration;
using PowerScheme.Model.Command;
using PowerScheme.Services;
using PowerScheme.Utility;
using PowerSchemeServiceAPI;
using RegistryManager;

namespace PowerScheme.Model.Menu.Settings;

public class ChangeLanguageCommand(
    IPowerSchemeService power,
    Action<string>? saveLanguage = null,
    Action? updateIcon = null,
    Action<string>? reportError = null) :
    IMenuEventHandlerCommand
{
    public void Execute(object sender, EventArgs e)
    {
        if (sender is ToolStripMenuItem { Tag: LanguageDefinition language })
        {
            try
            {
                Language.SetLanguage(language);
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or XmlException or ArgumentException)
            {
                (reportError ?? (message => ErrorLog.Write(message)))($"Cannot select language '{language.Code}': {exception}");
                return;
            }

            (saveLanguage ?? (code => RegistryService.SetLanguage(AppInfo.CompanyName, AppInfo.ProductName, code)))(language.Code);
            power.RenameTypicalSchemes();
            MenuLookup.RebuildMenuItems();

            (updateIcon ?? (() => DiRoot.GetService<IViewService>().UpdateIcon()))();
        }
    }
}
