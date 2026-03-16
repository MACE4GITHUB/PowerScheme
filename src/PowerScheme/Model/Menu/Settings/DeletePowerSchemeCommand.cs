using System;
using System.Windows.Forms;
using PowerScheme.Model.Command;
using PowerSchemeServiceAPI;
using PowerSchemeServiceAPI.Model;

namespace PowerScheme.Model.Menu.Settings;

public class DeletePowerSchemeCommand(
    IPowerSchemeService power) :
    IMenuEventHandlerCommand
{
    public void Execute(object sender, EventArgs e)
    {
        if (sender is ToolStripMenuItem { Tag: StatePowerScheme tag })
        {
            power.DeletePowerScheme(tag);
        }
    }
}
