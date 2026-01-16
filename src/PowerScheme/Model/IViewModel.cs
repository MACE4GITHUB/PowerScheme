using System;
using System.Drawing;
using System.Windows.Forms;
using PowerSchemeServiceAPI;

namespace PowerScheme.Model;

public interface IViewModel : IDisposable
{
    NotifyIcon NotifyIcon { get; }
    ContextMainMenu ContextLeftMenu { get; }
    ContextMainMenu ContextRightMenu { get; }
    IPowerSchemeService Power { get; }

    void BuildAllMenu();

    void ClearAllMenu();

    void UpdateIcon(Icon icon);

    void RemoveIcon();
}
