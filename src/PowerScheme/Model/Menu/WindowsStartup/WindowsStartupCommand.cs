using System;
using System.Windows.Forms;
using PowerScheme.Model.Command;
using RegistryManager;

namespace PowerScheme.Model.Menu.WindowsStartup;

internal class WindowsStartupCommand :
    IMenuEventHandlerCommand
{
    public void Execute(object? sender, EventArgs e)
    {
        if (sender is not ToolStripMenuItem { Tag: bool isChecked })
        {
            return;
        }

        RegistryService.SetStartup(!isChecked);
    }
}
