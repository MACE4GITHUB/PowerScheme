using PowerScheme.Forms;
using PowerScheme.Model.Command;

namespace PowerScheme.Model.Menu.IdleMonitoring;

internal class ShowIdleFormCommand :
    IMenuCommand
{
    public void Execute()
    {
        WindowManager.ShowForm<IdleForm>(true, 12);
    }
}
