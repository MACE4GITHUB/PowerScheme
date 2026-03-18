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
                           IdleThresholdInMilliseconds >= PollingIdleTimeInMilliseconds;

        var isValidIdleThresholdInMilliseconds =
            IdleThresholdInMilliseconds is >= MIN_IDLE_THRESHOLD_IN_MILLISECONDS and <= MAX_IDLE_THRESHOLD_IN_MILLISECONDS;

        var isValidPollingActiveTimeInMilliseconds =
            PollingActiveTimeInMilliseconds is >= MIN_POLLING_ACTIVE_TIME_IN_MILLISECONDS and <= MAX_POLLING_ACTIVE_TIME_IN_MILLISECONDS;

        var isValidPollingIdleTimeInMilliseconds =
            PollingIdleTimeInMilliseconds is >= MIN_POLLING_IDLE_TIME_IN_MILLISECONDS and <= MAX_POLLING_IDLE_TIME_IN_MILLISECONDS;

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
        new(
            DEFAULT_IDLE_THRESHOLD_IN_MILLISECONDS,
            DEFAULT_POLLING_ACTIVE_TIME_IN_MILLISECONDS,
            DEFAULT_POLLING_IDLE_TIME_IN_MILLISECONDS);

    public int IdleThresholdInMilliseconds { get; }

    public int PollingActiveTimeInMilliseconds { get; }

    public int PollingIdleTimeInMilliseconds { get; }
}
