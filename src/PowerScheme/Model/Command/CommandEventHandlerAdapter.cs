using System;

namespace PowerScheme.Model.Command;

internal class CommandEventHandlerAdapter(ICommand command)
{
    internal void Handle(object sender, EventArgs e)
    {
        switch (command)
        {
            case IMenuCommand menuCommand:
                menuCommand.Execute();
                break;
            case IMenuEventHandlerCommand menuEventHandlerCommand:
                menuEventHandlerCommand.Execute(sender, e);
                break;
        }
    }
}
