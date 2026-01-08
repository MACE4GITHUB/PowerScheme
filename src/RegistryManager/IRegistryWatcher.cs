namespace RegistryManager;

using System;
using System.IO;
using EventsArgs;
using Model;

public interface IRegistryWatcher
{
    /// <summary>
    /// Occurs when the specified registry key has changed.
    /// </summary>
    event EventHandler<RegistryChangedEventArgs> Changed;

    /// <summary>
    /// Occurs when the access to the registry fails.
    /// </summary>
    event ErrorEventHandler Error;

    /// <summary>
    /// Gets or sets the <see cref="RegChangeNotifyFilter">RegChangeNotifyFilter</see>.
    /// </summary>
    RegChangeNotifyFilter RegChangeNotifyFilter { get; set; }

    /// <summary>
    /// <b>true</b> if this <see cref="RegistryWatcher"/> object is currently monitoring;
    /// otherwise, <b>false</b>.
    /// </summary>
    bool IsMonitoring { get; }

    /// <summary>
    /// Start monitoring.
    /// </summary>
    void Start();

    /// <summary>
    /// Stops the monitoring thread.
    /// </summary>
    void Stop();
}