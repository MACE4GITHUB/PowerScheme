using System;
using Languages;
using static PowerScheme.Addins.IdleMonitoring.Constants;

namespace PowerScheme.Model.Options;

internal class IdleOptions(
    int idleThresholdMs = DEFAULT_IDLE_THRESHOLD_IN_MILLISECONDS,
    int pollingActiveTimeMs = DEFAULT_POLLING_ACTIVE_TIME_IN_MILLISECONDS,
    int pollingIdleTimeMs = DEFAULT_POLLING_IDLE_TIME_IN_MILLISECONDS)
{
    private const int IDLE_THRESHOLD_SCALE = 200;

    public int IdleThreshold { get; set; } =
        idleThresholdMs * IDLE_THRESHOLD_SCALE / MAX_IDLE_THRESHOLD_IN_MILLISECONDS;

    public int IdleThresholdMs =>
        MAX_IDLE_THRESHOLD_IN_MILLISECONDS * IdleThreshold / IDLE_THRESHOLD_SCALE;

    public string IdleThresholdMsFormated => FormatMinutesSeconds(IdleThresholdMs);

    public int PollingActiveTime { get; set; } = pollingActiveTimeMs;

    public int PollingActiveTimeMs => PollingActiveTime;

    public string PollingActiveTimeMsFormated => FormatSeconds(PollingActiveTimeMs);

    public int PollingIdleTime { get; set; } = pollingIdleTimeMs;

    public int PollingIdleTimeMs => PollingIdleTime;

    public string PollingIdleTimeMsFormated => FormatSeconds(PollingIdleTimeMs);

    private static string FormatMinutesSeconds(int milliseconds)
    {
        var ts = TimeSpan.FromMilliseconds(milliseconds);
        var minutes = ts.Minutes > 0
            ? $"{ts.Minutes} {Language.Current.Minutes}"
            : "";

        var seconds = ts.Seconds != 0
            ? $" {ts.Seconds} {Language.Current.Seconds}"
            : "";

        return $"{minutes}{seconds}";
    }

    private static string FormatSeconds(int milliseconds)
    {
        var ts = RoundToOneDecimal(milliseconds / 1000f);

        var seconds = ts != 0
            ? $" {ts} {Language.Current.Seconds}"
            : "";

        return $"{seconds}";
    }

    public static float RoundToOneDecimal(float value) =>
        (float)Math.Round(value, 1, MidpointRounding.AwayFromZero);
}