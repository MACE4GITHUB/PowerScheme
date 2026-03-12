using System;

namespace PowerScheme.Services;

internal interface IViewService : IDisposable
{
    void Start();

    void Stop();
}
