using System.ComponentModel;

namespace PowerScheme.Model;

using Utility;
using PowerSchemeServiceAPI;
using System.Windows.Forms;

public abstract class ContextMainMenu : ContextMenuStrip
{
    protected IPowerSchemeService Power;

    protected ContextMainMenu(IContainer components, IPowerSchemeService power): base(components)
    {
        Power = power;
    }

    public void BuildMenu()
    {
        this.InvokeIfRequired(BuildContextMenu);
    }

    public abstract void UpdateMenu();

    public abstract void ClearMenu();

    protected abstract void BuildContextMenu();

}