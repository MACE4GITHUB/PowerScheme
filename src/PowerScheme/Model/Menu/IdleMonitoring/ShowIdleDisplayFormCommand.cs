using PowerScheme.Forms;
using PowerScheme.Model.Command;
using static PowerScheme.Forms.WindowManager;

namespace PowerScheme.Model.Menu.IdleMonitoring;

internal class ShowIdleDisplayFormCommand :
    IMenuCommand
{
    public void Execute()
    {
        ShowForm<IdleDisplayForm>(true, 12);
    }
}
