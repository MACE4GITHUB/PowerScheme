using System;
using System.Threading;
using System.Threading.Tasks;

namespace PowerScheme.Addins.IdleMonitoring;

public class IdleMonitor(
    IIdleDetector detector) : IIdleMonitor
{
    private readonly object _sync = new();
    private CancellationTokenSource? _cts;
    private Task? _monitoringTask;

    public event EventHandler? OnIdle;
    public event EventHandler? OnActive;

    public async Task StartAsync(
        IdleMonitorOption idleMonitorOption)
    {
        lock (_sync)
        {
            if (_monitoringTask is { IsCompleted: false })
            {
                throw new InvalidOperationException("Monitoring has already been started.");
            }

            _cts = new CancellationTokenSource();
            _monitoringTask = MonitorLoopAsync(idleMonitorOption, _cts.Token);
        }

        await Task.Yield();
    }

    public void Stop()
    {
        lock (_sync)
        {
            _cts?.Cancel();
        }
    }

    public async Task WaitForStopAsync()
    {
        Task? task;
        lock (_sync)
        {
            task = _monitoringTask;
        }

        if (task != null)
        {
            await task;
        }
    }

    private async Task MonitorLoopAsync(
        IdleMonitorOption idleMonitorOption,
        CancellationToken token)
    {
        if (!idleMonitorOption.IsValid)
        {
            idleMonitorOption = IdleMonitorOption.Default;
        }

        var idleThresholdInMilliseconds = TimeSpan.FromMilliseconds(idleMonitorOption.IdleThresholdInMilliseconds);
        var pollingIdleTimeInMilliseconds = TimeSpan.FromMilliseconds(idleMonitorOption.PollingIdleTimeInMilliseconds);
        var pollingActiveTimeInMilliseconds = TimeSpan.FromMilliseconds(idleMonitorOption.PollingActiveTimeInMilliseconds);

        var isIdle = false;

        try
        {
            while (!token.IsCancellationRequested)
            {
                var idle = detector.GetIdleTime();

                switch (isIdle)
                {
                    case false when idle >= idleThresholdInMilliseconds:
                        isIdle = true;
                        RaiseEvent(OnIdle);
                        break;
                    case true when idle < idleThresholdInMilliseconds:
                        isIdle = false;
                        RaiseEvent(OnActive);
                        break;
                }

                var delay = isIdle
                    ? pollingIdleTimeInMilliseconds
                    : pollingActiveTimeInMilliseconds;

                await Task.Delay(delay, token);
            }
        }
        catch (OperationCanceledException)
        {
            // Do nothing
        }
        catch
        {
            // Do logging here if needed
        }
        finally
        {
            lock (_sync)
            {
                _cts?.Dispose();
                _cts = null;
                _monitoringTask = null;
            }
        }
    }

    private void RaiseEvent(EventHandler? handler)
    {
        try
        {
            handler?.Invoke(this, EventArgs.Empty);
        }
        catch
        {
            // Do logging here if needed
        }
    }
}
