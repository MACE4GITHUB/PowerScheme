using PowerScheme.Model.Command;
using PowerSchemeServiceAPI;

namespace PowerScheme.Model.Menu.Settings;

public class DeleteTypicalPowerSchemesCommand(
    IPowerSchemeService power) :
    IMenuCommand
{
    public void Execute() =>
        power.Watchers.RaiseActionWithoutWatchers(power.DeleteAllTypicalScheme);
}
