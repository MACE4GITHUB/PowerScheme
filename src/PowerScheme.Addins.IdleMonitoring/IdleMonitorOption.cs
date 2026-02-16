using static PowerScheme.Addins.IdleMonitoring.Constants;

namespace PowerScheme.Addins.IdleMonitoring;

public sealed class IdleMonitorOption
{
    public IdleMonitorOption(
        int idleThresholdInMilliseconds,
        int pollingActiveTimeInMilliseconds,
        int pollingIdleTimeInMilliseconds)
    {
        IdleThresholdInMilliseconds = idleThresholdInMilliseconds;
        PollingActiveTimeInMilliseconds = pollingActiveTimeInMilliseconds;
        PollingIdleTimeInMilliseconds = pollingIdleTimeInMilliseconds;

        var isValidChain = IdleThresholdInMilliseconds >= PollingActiveTimeInMilliseconds &&
            PollingActiveTimeInMilliseconds >= PollingIdleTimeInMilliseconds;

        var isValidIdleThresholdInMilliseconds =
            MIN_IDLE_THRESHOLD_IN_MILLISECONDS <= IdleThresholdInMilliseconds &&
            IdleThresholdInMilliseconds <= MAX_IDLE_THRESHOLD_IN_MILLISECONDS;

        var isValidPollingActiveTimeInMilliseconds =
            MIN_POLLING_ACTIVE_TIME_IN_MILLISECONDS <= PollingActiveTimeInMilliseconds &&
            PollingActiveTimeInMilliseconds <= MAX_POLLING_ACTIVE_TIME_IN_MILLISECONDS;

        var isValidPollingIdleTimeInMilliseconds =
            MIN_POLLING_IDLE_TIME_IN_MILLISECONDS <= PollingIdleTimeInMilliseconds &&
            PollingIdleTimeInMilliseconds <= MAX_POLLING_IDLE_TIME_IN_MILLISECONDS;

        if (!isValidChain || !isValidIdleThresholdInMilliseconds ||
            !isValidPollingActiveTimeInMilliseconds || !isValidPollingIdleTimeInMilliseconds)
        {
            IsValid = false;
            return;
        }

        IsValid = true;
    }

    public bool IsValid { get; }

    public static IdleMonitorOption Default { get; } =
        new IdleMonitorOption(
            DEFAULT_IDLE_THRESHOLD_IN_MILLISECONDS,
            DEFAULT_POLLING_ACTIVE_TIME_IN_MILLISECONDS,
            DEFAULT_POLLING_IDLE_TIME_IN_MILLISECONDS);

    public int IdleThresholdInMilliseconds { get; }

    public int PollingActiveTimeInMilliseconds { get; }

    public int PollingIdleTimeInMilliseconds { get; }
}
