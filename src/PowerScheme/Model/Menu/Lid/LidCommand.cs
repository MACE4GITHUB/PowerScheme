using System;
using System.Windows.Forms;
using PowerScheme.Model.Command;
using PowerSchemeServiceAPI;

namespace PowerScheme.Model.Menu.Lid;

using static MenuLookup;

internal class LidCommand(
    IPowerSchemeService power) :
    IMenuEventHandlerCommand
{
    public void Execute(object? sender, EventArgs e)
    {
        if (sender is not ToolStripMenuItem { Tag: Lid value })
        {
            return;
        }

        power.SetLid((int)value);
    }
}
