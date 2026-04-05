namespace PowerScheme.Addins.IdleMonitoring;

public static class Constants
{
    public const int DEFAULT_IDLE_THRESHOLD_IN_MILLISECONDS = 60_000;
    public const int MIN_IDLE_THRESHOLD_IN_MILLISECONDS = 5_000;
    public const int MAX_IDLE_THRESHOLD_IN_MILLISECONDS = 1_000_000; // 16m 40s

    public const int DEFAULT_POLLING_ACTIVE_TIME_IN_MILLISECONDS = 4_500;
    public const int MIN_POLLING_ACTIVE_TIME_IN_MILLISECONDS = 1_000;
    public const int MAX_POLLING_ACTIVE_TIME_IN_MILLISECONDS = 5_000;

    public const int DEFAULT_POLLING_IDLE_TIME_IN_MILLISECONDS = 2_500;
    public const int MIN_POLLING_IDLE_TIME_IN_MILLISECONDS = 1_000;
    public const int MAX_POLLING_IDLE_TIME_IN_MILLISECONDS = 5_000;

    public const int DEFAULT_TURN_OFF_DISPLAY_IN_SECONDS = 0;
    public const int MIN_TURN_OFF_DISPLAY_IN_SECONDS = 0;
    public const int MAX_TURN_OFF_DISPLAY_IN_SECONDS = 18_000; // 5h

    public const int DEFAULT_TURN_OFF_LOCK_DISPLAY_IN_SECONDS = 0;
    public const int MIN_TURN_OFF_LOCK_DISPLAY_IN_SECONDS = 0;
    public const int MAX_TURN_OFF_LOCK_DISPLAY_IN_SECONDS = 18_000; // 5h
}
