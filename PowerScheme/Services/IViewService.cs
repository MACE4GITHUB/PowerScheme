using System;
using System.Threading;

namespace PowerScheme.Services
{
    public interface IViewService: IDisposable
    {
        void Start();

        void Stop();
    }
}