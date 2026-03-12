using PowerScheme.Model.Command;
using PowerSchemeServiceAPI;

namespace PowerScheme.Model.Menu.Settings;

public class CreateTypicalPowerSchemesCommand(
    IPowerSchemeService power) :
    IMenuCommand
{
    public void Execute() =>
        power.CreateTypicalSchemes();
}
