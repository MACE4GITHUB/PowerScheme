using PowerScheme.Forms;
using PowerScheme.Model.Command;
using static PowerScheme.Forms.WindowManager;

namespace PowerScheme.Model.Menu.IdleMonitoring;

internal class ShowIdleSleepFormCommand :
    IMenuCommand
{
    public void Execute()
    {
        ShowForm<IdleSleepForm>(true, 12);
    }
}
