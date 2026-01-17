using System.ComponentModel;
using System.Windows.Forms;
using PowerScheme.Utility;
using PowerSchemeServiceAPI;

namespace PowerScheme.Model;

public abstract class ContextMainMenu(
    IContainer components,
    IPowerSchemeService power) :
    ContextMenuStrip(components)
{
    protected IPowerSchemeService? Power = power;

    public void BuildMenu()
    {
        this.InvokeIfRequired(BuildContextMenu);
    }

    public abstract void UpdateMenu();

    public abstract void ClearMenu();

    protected abstract void BuildContextMenu();
}
