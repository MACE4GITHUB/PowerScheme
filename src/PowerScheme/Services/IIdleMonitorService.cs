using System;
using System.Threading.Tasks;

namespace PowerScheme.Services;

public interface IIdleMonitorService : IDisposable
{
    void Start();
    void Stop();
    Task RestartAsync();
}
