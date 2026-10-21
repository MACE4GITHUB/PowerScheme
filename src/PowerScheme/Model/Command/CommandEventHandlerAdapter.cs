using System;

namespace PowerScheme.Model.Command;

internal class CommandEventHandlerAdapter(ICommand command)
{
    internal ICommand Command { get; set; } = command;

    internal void Handle(object sender, EventArgs e)
    {
        switch (Command)
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
