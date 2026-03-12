using System;
using System.Windows.Forms;
using PowerScheme.Model.Command;
using PowerScheme.Properties;
using RegistryManager;

namespace PowerScheme.Model.Menu.Hibernate;

internal class HibernateCommand :
    IMenuEventHandlerCommand
{
    public void Execute(object? sender, EventArgs e)
    {
        if (sender is not ToolStripMenuItem { Tag: bool isChecked })
        {
            return;
        }

        var value = isChecked ? 0 : 1;
        RegistryService.SetHibernateOption(Resources.ResourceManager, value);
    }
}
