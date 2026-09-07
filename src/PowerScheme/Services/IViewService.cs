using System;

namespace PowerScheme.Services;

internal interface IViewService : IDisposable
{
    void Start();

    void Stop();

    /// <summary>
    /// Updates the tray icon and its tooltip from the current active power scheme.
    /// </summary>
    void UpdateIcon();
}
