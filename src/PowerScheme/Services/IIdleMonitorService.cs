using System;

namespace PowerScheme.Services;

public interface IIdleMonitorService : IDisposable
{
    void Start();
    void Stop();
}
