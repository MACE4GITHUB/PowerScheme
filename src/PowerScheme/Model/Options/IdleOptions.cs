using System;
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

    public string IdleThresholdMsFormated => OptionsHelper.FormatMinutesSeconds(TimeSpan.FromMilliseconds(IdleThresholdMs));

    public int PollingActiveTime { get; set; } = pollingActiveTimeMs;

    public int PollingActiveTimeMs => PollingActiveTime;

    public string PollingActiveTimeMsFormated => OptionsHelper.FormatSeconds(TimeSpan.FromMilliseconds(PollingActiveTimeMs));

    public int PollingIdleTime { get; set; } = pollingIdleTimeMs;

    public int PollingIdleTimeMs => PollingIdleTime;

    public string PollingIdleTimeMsFormated => OptionsHelper.FormatSeconds(TimeSpan.FromMilliseconds(PollingIdleTimeMs));
}
