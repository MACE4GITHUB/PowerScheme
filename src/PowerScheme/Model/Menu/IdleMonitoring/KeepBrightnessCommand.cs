using System;
using System.Windows.Forms;
using PowerScheme.Model.Command;
using RegistryManager;

namespace PowerScheme.Model.Menu.IdleMonitoring;

internal class KeepBrightnessCommand :
    IMenuEventHandlerCommand
{
    public void Execute(object? sender, EventArgs e)
    {
        if (sender is not ToolStripMenuItem { Tag: bool isChecked })
        {
            return;
        }

        var value = isChecked ? 0 : 1;

        RegistryService.SetKeepBrightness(AppInfo.CompanyName, AppInfo.ProductName, value);
    }
}
