using System.Windows.Forms;
using PowerScheme.Model.Command;

namespace PowerScheme.Model.Menu.Quit;

internal class QuitCommand :
    IMenuCommand
{
    public void Execute() =>
        Application.Exit();
}
