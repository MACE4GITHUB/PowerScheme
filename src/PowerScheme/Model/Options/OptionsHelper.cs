using System;
using Languages;

namespace PowerScheme.Model.Options;

internal static class OptionsHelper
{
    public static string FormatHoursMinutesSeconds(TimeSpan ts)
    {
        var hours = ts.Hours > 0
            ? $"{ts.Hours} {Language.Current.Hours}"
            : "";

        return $"{hours} {FormatMinutesSeconds(ts)}";
    }

    public static string FormatMinutesSeconds(TimeSpan ts)
    {
        var minutes = ts.Minutes > 0
            ? $"{ts.Minutes} {Language.Current.Minutes}"
            : "";

        var seconds = ts.Seconds != 0
            ? $"{ts.Seconds} {Language.Current.Seconds}"
            : "";

        return $"{minutes} {seconds}";
    }

    public static string FormatSeconds(TimeSpan ts)
    {
        var tts = ts.Seconds + RoundToOneDecimal(ts.Milliseconds / 1000f);

        var seconds = tts != 0
            ? $"{tts} {Language.Current.Seconds}"
            : "";

        return $"{seconds}";
    }

    private static float RoundToOneDecimal(float value) =>
        (float)Math.Round(value, 1, MidpointRounding.AwayFromZero);
}
