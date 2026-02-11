using System;
using System.Collections.Generic;
using RegistryManager.Api;
using RegistryManager.EventsArgs;
using static RegistryManager.Dpi.DpiRegistryParams;
using static RegistryManager.Dpi.DpiRegistryWatcher;

namespace RegistryManager.Dpi;

public sealed class DpiWatchers : IDisposable
{
    private readonly Dictionary<string, IRegistryWatcher> _monitorRegistryWatchers = [];
    private readonly IRegistryWatcher _scaleFactorsRegistryWatcher = ScaleFactorsRegistryWatcher();

    public event EventHandler<RegistryChangedEventArgs>? Changed;

    public void Start()
    {
        if (!_scaleFactorsRegistryWatcher.IsMonitoring)
        {
            _scaleFactorsRegistryWatcher.Changed += ScaleFactorsRegistryWatcher_Changed;
            _scaleFactorsRegistryWatcher.Start();
        }

        MonitorRegistryWatchersStart();
    }

    public void Stop()
    {
        if (_scaleFactorsRegistryWatcher.IsMonitoring)
        {
            _scaleFactorsRegistryWatcher.Stop();
            _scaleFactorsRegistryWatcher.Changed -= ScaleFactorsRegistryWatcher_Changed;
        }

        MonitorRegistryWatchersStop();
    }

    public void MonitorRegistryWatchersStart()
    {
        if (_monitorRegistryWatchers.Count > 0)
        {
            return;
        }

        foreach (var param in GetMonitorRegistryParams())
        {
            _monitorRegistryWatchers[param.Section] = MonitorWatcher(param);
            _monitorRegistryWatchers[param.Section].Changed += MonitorRegistryWatchers_Changed;
            _monitorRegistryWatchers[param.Section].Start();
        }
    }

    public void MonitorRegistryWatchersStop()
    {
        foreach (var monitor in _monitorRegistryWatchers.Values)
        {
            if (monitor.IsMonitoring)
            {
                monitor.Stop();
            }

            monitor.Changed -= MonitorRegistryWatchers_Changed;
        }

        _monitorRegistryWatchers.Clear();
    }

    public void Dispose()
    {
        if (_monitorRegistryWatchers.Count > 0)
        {
            MonitorRegistryWatchersStop();
        }
    }

    private void ScaleFactorsRegistryWatcher_Changed(object sender, RegistryChangedEventArgs e)
    {
        MonitorRegistryWatchersStop();
        MonitorRegistryWatchersStart();
    }

    private void MonitorRegistryWatchers_Changed(object sender, RegistryChangedEventArgs e)
    {
        if (e.Current.Value is Result<int> currentValue && currentValue.IsSuccess &&
            e.Previous.Value is Result<int> previousValue && previousValue.IsSuccess &&
            currentValue.Data != previousValue.Data)
        {
            Changed?.Invoke(sender, e);
        }
    }
}
