using System.ComponentModel;
using System.Windows.Forms;
using PowerScheme.Utility;
using PowerSchemeServiceAPI;

namespace PowerScheme.Model;

internal abstract class ContextMainMenu(
    IContainer components,
    IPowerSchemeService power) :
    ContextMenuStrip(components)
{
    protected IPowerSchemeService Power { get; } = power;

    internal void BuildMenu()
    {
        this.InvokeIfRequired(BuildContextMenu);
    }

    internal abstract void UpdateMenu();

    internal abstract void ClearMenu();

    protected abstract void BuildContextMenu();
}
