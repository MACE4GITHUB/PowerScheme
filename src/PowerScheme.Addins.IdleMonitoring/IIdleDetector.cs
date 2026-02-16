using System;

namespace PowerScheme.Addins.IdleMonitoring;

public interface IIdleDetector
{
    TimeSpan GetIdleTime();
}
