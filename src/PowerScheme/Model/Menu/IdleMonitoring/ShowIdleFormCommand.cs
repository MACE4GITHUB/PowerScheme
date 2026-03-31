using PowerScheme.Forms;
using PowerScheme.Model.Command;
using static PowerScheme.Forms.WindowManager;

namespace PowerScheme.Model.Menu.IdleMonitoring;

internal class ShowIdleFormCommand :
    IMenuCommand
{
    public void Execute()
    {
        ShowForm<IdleForm>(true, 12);
    }
}
