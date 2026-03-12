using System;
using System.Windows.Forms;
using PowerScheme.Model.Command;
using PowerSchemeServiceAPI;
using PowerSchemeServiceAPI.Model;

namespace PowerScheme.Model.Menu.PowerSchemes;

internal class PowerSchemeCommand(
    IPowerSchemeService power) :
    IMenuEventHandlerCommand
{
    public void Execute(object? sender, EventArgs e)
    {
        if (sender is not ToolStripMenuItem { Tag: StatePowerScheme statePowerScheme })
        {
            return;
        }

        power.SetActivePowerScheme(statePowerScheme.PowerScheme);
    }
}
