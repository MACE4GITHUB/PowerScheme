using PowerScheme.Model.Command;
using RunAs.Common.Utils;

namespace PowerScheme.Model.Menu.Settings;

public class OpenPowerSchemesPanelCommand :
    IMenuCommand
{
    private const string POWER_CFG_CPL = "powercfg.cpl";

    public void Execute() =>
        UacHelper.AttemptPrivilegeEscalation(POWER_CFG_CPL);
}
