using PowerScheme.Model.Command;
using PowerSchemeServiceAPI;

namespace PowerScheme.Model.Menu.Settings;

public class RestoreDefaultPowerSchemesCommand(
    IPowerSchemeService power) :
    IMenuCommand
{
    public void Execute() =>
        power.RestoreDefaultPowerSchemes();
}
