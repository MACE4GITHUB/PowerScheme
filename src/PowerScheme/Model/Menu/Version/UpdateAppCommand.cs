using PowerScheme.Model.Command;
using PowerScheme.Services;

namespace PowerScheme.Model.Menu.Version;

internal class UpdateAppCommand(
    IUpdateService updateService) :
    IMenuCommand
{
    public void Execute() =>
        updateService.Update();
}
