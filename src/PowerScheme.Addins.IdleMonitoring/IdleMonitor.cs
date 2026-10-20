using System;
using System.Threading;
using System.Threading.Tasks;

namespace PowerScheme.Addins.IdleMonitoring;

public class IdleMonitor(
    IIdleDetector detector,
    Action<string, Exception>? reportError = null) : IIdleMonitor
{
    private readonly object _sync = new();
    private CancellationTokenSource? _cts;
    private Task? _monitoringTask;
    private readonly System.Collections.Generic.Dictionary<string, DateTime> _lastErrors = new();
    internal Func<TimeSpan, CancellationToken, Task> DelayAsync { get; set; } = Task.Delay;

    public event EventHandler? OnIdle;
    public event EventHandler? OnActive;
    public event EventHandler<IdleMonitorFaultEventArgs>? Faulted;

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
        var activePending = true;
        var initialSample = true;
        Exception? failure = null;

        try
        {
            while (!token.IsCancellationRequested)
            {
                TimeSpan idle;
                try
                {
                    idle = detector.GetIdleTime();
                }
                catch (Exception exception) when (!token.IsCancellationRequested)
                {
                    ReportError("ReadIdleTime", exception);
                    await DelayAsync(pollingActiveTimeInMilliseconds, token);
                    continue;
                }

                if (!isIdle && idle >= idleThresholdInMilliseconds)
                {
                    isIdle = true;
                    activePending = true;
                    RaiseEvent(OnIdle, EventArgs.Empty, "EnterIdle");
                }
                else if (idle < idleThresholdInMilliseconds && activePending)
                {
                    isIdle = false;
                    activePending = !RaiseEvent(OnActive, new IdleMonitorActiveEventArgs(initialSample), "RestoreActive");
                }
                initialSample = false;

                var delay = isIdle
                    ? pollingIdleTimeInMilliseconds
                    : pollingActiveTimeInMilliseconds;

                await DelayAsync(delay, token);
            }
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            failure = exception;
            ReportError("MonitorLoop", exception);
        }
        finally
        {
            lock (_sync)
            {
                _cts?.Dispose();
                _cts = null;
            }

            if (failure != null)
            {
                try
                {
                    Faulted?.Invoke(this, new IdleMonitorFaultEventArgs(failure));
                }
                catch (Exception exception)
                {
                    ReportError("FaultNotification", exception);
                }
            }
        }
    }

    private bool RaiseEvent(EventHandler? handler, EventArgs args, string stage)
    {
        try
        {
            handler?.Invoke(this, args);
            return true;
        }
        catch (Exception exception)
        {
            ReportError(stage, exception);
            return false;
        }
    }

    private void ReportError(string stage, Exception exception)
    {
        var now = DateTime.UtcNow;
        if (_lastErrors.TryGetValue(stage, out var previous) && now - previous < TimeSpan.FromMinutes(1))
        {
            return;
        }

        _lastErrors[stage] = now;
        try
        {
            if (reportError != null)
            {
                reportError(stage, exception);
            }
            else
            {
                System.Diagnostics.Trace.TraceError("Idle monitoring error ({0}): {1}", stage, exception);
            }
        }
        catch (Exception)
        {
            System.Diagnostics.Trace.TraceError("Idle monitoring error ({0}): {1}", stage, exception);
        }
    }
}
