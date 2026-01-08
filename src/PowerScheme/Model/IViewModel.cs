using System;
using System.Windows.Forms;
using PowerScheme.Services;
using PowerSchemeServiceAPI;

namespace PowerScheme.Model;

public interface IViewModel: IDisposable
{
    NotifyIcon NotifyIcon { get; }
    ContextMainMenu ContextLeftMenu { get; }
    ContextMainMenu ContextRightMenu { get; }
    IPowerSchemeService Power { get; }

    void BuildAllMenu();

    void ClearAllMenu();
}