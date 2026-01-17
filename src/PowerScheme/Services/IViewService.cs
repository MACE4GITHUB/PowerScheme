using System;

namespace PowerScheme.Services;

public interface IViewService : IDisposable
{
    void Start();

    void Stop();
}
