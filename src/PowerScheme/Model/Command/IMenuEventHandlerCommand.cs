using System;

namespace PowerScheme.Model.Command;

public interface IMenuEventHandlerCommand : ICommand
{
    void Execute(object sender, EventArgs e);
}
