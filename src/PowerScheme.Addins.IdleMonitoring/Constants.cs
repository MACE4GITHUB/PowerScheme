namespace PowerScheme.Addins.IdleMonitoring;

public static class Constants
{
    public static readonly int DEFAULT_IDLE_THRESHOLD_IN_MILLISECONDS = 5_000;
    public static readonly int DEFAULT_POLLING_ACTIVE_TIME_IN_MILLISECONDS = 4_500;
    public static readonly int DEFAULT_POLLING_IDLE_TIME_IN_MILLISECONDS = 2_500;

    public static readonly int MIN_IDLE_THRESHOLD_IN_MILLISECONDS = 1_000;
    public static readonly int MIN_POLLING_ACTIVE_TIME_IN_MILLISECONDS = 800;
    public static readonly int MIN_POLLING_IDLE_TIME_IN_MILLISECONDS = 500;

    public static readonly int MAX_IDLE_THRESHOLD_IN_MILLISECONDS = 10_800_000; // 3h
    public static readonly int MAX_POLLING_ACTIVE_TIME_IN_MILLISECONDS = 60_000;// 1m
    public static readonly int MAX_POLLING_IDLE_TIME_IN_MILLISECONDS = 5_000;// 5s
}
