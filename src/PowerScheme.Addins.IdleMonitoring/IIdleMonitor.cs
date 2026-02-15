using System;
using System.Threading.Tasks;

namespace PowerScheme.Addins.IdleMonitoring;

/// <summary>
/// Defines a contract for monitoring and responding to idle and active states within an application.
/// </summary>
/// <example>
/// var detector = new Win32IdleDetector();
/// var monitor = new IdleMonitor(detector, TimeSpan.FromMinutes(2));
/// monitor.OnIdle += (s, e) =&gt; Console.WriteLine("The PC is idle");
/// monitor.OnActive += (s, e) =&gt; Console.WriteLine("The user is active again");
/// Console.WriteLine("Monitoring has started. Enter to stop.");
/// await monitor.StartAsync();
/// Console.ReadLine();
/// monitor.Stop();
/// await monitor.WaitForStopAsync();
/// </example>>
public interface IIdleMonitor
{
    event EventHandler? OnIdle;
    event EventHandler? OnActive;
    Task StartAsync(TimeSpan threshold);
    void Stop();
    Task WaitForStopAsync();
}
