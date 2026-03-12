using System;
using System.Windows.Forms;
using PowerScheme.Model.Command;
using RegistryManager;

namespace PowerScheme.Model.Menu.IdleMonitoring;

internal class IdleMonitoringCommand :
    IMenuEventHandlerCommand
{
    public void Execute(object? sender, EventArgs e)
    {
        if (sender is not ToolStripMenuItem { Tag: Guid value })
        {
            return;
        }

        RegistryService.SetIdlePowerScheme(AppInfo.CompanyName, AppInfo.ProductName, value);
    }
}
