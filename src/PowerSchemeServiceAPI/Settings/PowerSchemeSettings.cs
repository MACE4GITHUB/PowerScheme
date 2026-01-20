using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static PowerSchemeServiceAPI.SettingSchemeLookup;

namespace PowerSchemeServiceAPI.Settings;

internal class PowerSchemeSettings(SettingScheme settingScheme)
{
    public Guid PowerSchemeGuid { get; } = SettingSchemes
            .Where(p => p.Key == settingScheme)
            .Select(p => p.Value.Guid).FirstOrDefault();

    public PowerSchemeProcessorThrottle ProcessorThrottle(RangeValues values) =>
        new(PowerSchemeGuid, values.MinState, values.MaxState);

    public PowerSchemeSleep Sleep(StateValues values) => new(PowerSchemeGuid, values.State);

    public PowerSchemeTurnOffDisplay TurnOffDisplay(StateValues values) => new(PowerSchemeGuid, values.State);

    public PowerSchemeWiFi WiFi(StateValues values) => new(PowerSchemeGuid, values.State);

    public PowerSchemeMultimediaPlay MultimediaPlay(StateValues values) => new(PowerSchemeGuid, values.State);

    public PowerSchemeMultimediaShare MultimediaShare(StateValues values) => new(PowerSchemeGuid, values.State);

    public PowerSchemeMultimediaQuality MultimediaQuality(StateValues values) => new(PowerSchemeGuid, values.State);

    public void ApplyDefaultValues()
    {
        var powerSchemeDefaultSettings = new PowerSchemeDefaultSettings(settingScheme);
        var ds = powerSchemeDefaultSettings.Settings;

        var typeSetting = typeof(PowerSchemeSetting);
        foreach (var name in Enum.GetNames(typeSetting))
        {
            var set = (PowerSchemeSetting)Enum.Parse(typeof(PowerSchemeSetting), name);
            var mi = typeof(PowerSchemeSettings).GetMethod(name, BindingFlags.Instance | BindingFlags.Public);
            var res = mi?.Invoke(this, [ds[set]]);
            var applicable = res as IApplicable;
            try
            {
                applicable?.ApplyValues();
            }
            catch (Exception)
            {
                // Do nothing. Setting is not exists.
            }
        }
    }
}

internal class PowerSchemeDefaultSettings
{
    public PowerSchemeDefaultSettings(SettingScheme settingScheme)
    {
        var defaultSetting = settingScheme switch
        {
            SettingScheme.Stable => SettingsStable,
            SettingScheme.Media => SettingsMedia,
            SettingScheme.Simple => SettingsSimple,
            SettingScheme.Extreme => SettingsExtreme,
            _ => throw new ArgumentOutOfRangeException(nameof(settingScheme), settingScheme, null)
        };

        Settings = new Dictionary<PowerSchemeSetting, Values>
        {
            {PowerSchemeSetting.ProcessorThrottle, new RangeValues( new PowerSchemeDcAcValues(defaultSetting[PowerSchemeSetting.ProcessorThrottle][0], defaultSetting[PowerSchemeSetting.ProcessorThrottle][1]),
                new PowerSchemeDcAcValues(defaultSetting[PowerSchemeSetting.ProcessorThrottle][2], defaultSetting[PowerSchemeSetting.ProcessorThrottle][3]))},
            {PowerSchemeSetting.TurnOffDisplay, new StateValues( new PowerSchemeDcAcValues(defaultSetting[PowerSchemeSetting.TurnOffDisplay][0], defaultSetting[PowerSchemeSetting.TurnOffDisplay][1]))},
            {PowerSchemeSetting.Sleep, new StateValues( new PowerSchemeDcAcValues(defaultSetting[PowerSchemeSetting.Sleep][0], defaultSetting[PowerSchemeSetting.Sleep][1]))},
            {PowerSchemeSetting.WiFi, new StateValues( new PowerSchemeDcAcValues(defaultSetting[PowerSchemeSetting.WiFi][0], defaultSetting[PowerSchemeSetting.WiFi][1]))},
            {PowerSchemeSetting.MultimediaPlay, new StateValues( new PowerSchemeDcAcValues(defaultSetting[PowerSchemeSetting.MultimediaPlay][0], defaultSetting[PowerSchemeSetting.MultimediaPlay][1]))},
            {PowerSchemeSetting.MultimediaShare, new StateValues( new PowerSchemeDcAcValues(defaultSetting[PowerSchemeSetting.MultimediaShare][0], defaultSetting[PowerSchemeSetting.MultimediaShare][1]))},
            {PowerSchemeSetting.MultimediaQuality, new StateValues( new PowerSchemeDcAcValues(defaultSetting[PowerSchemeSetting.MultimediaQuality][0], defaultSetting[PowerSchemeSetting.MultimediaQuality][1]))}
        };
    }

    public Dictionary<PowerSchemeSetting, Values> Settings { get; }

    private static Dictionary<PowerSchemeSetting, int[]> SettingsStable
        => new()
        {
            {PowerSchemeSetting.ProcessorThrottle, [5,85,60,85] },
            {PowerSchemeSetting.TurnOffDisplay, [300,7200] },
            {PowerSchemeSetting.Sleep, [900,10800] },
            {PowerSchemeSetting.WiFi, [2,2] },
            {PowerSchemeSetting.MultimediaPlay, [1,1] },
            {PowerSchemeSetting.MultimediaShare, [2,2] },
            {PowerSchemeSetting.MultimediaQuality, [1,1] }
        };

    private static Dictionary<PowerSchemeSetting, int[]> SettingsMedia
        => new()
        {
            {PowerSchemeSetting.ProcessorThrottle, [5,5,60,85] },
            {PowerSchemeSetting.TurnOffDisplay, [300,7200] },
            {PowerSchemeSetting.Sleep, [1800,10800] },
            {PowerSchemeSetting.WiFi, [2,3] },
            {PowerSchemeSetting.MultimediaPlay, [2,1] },
            {PowerSchemeSetting.MultimediaShare, [0,1] },
            {PowerSchemeSetting.MultimediaQuality, [1,1] }
        };

    private static Dictionary<PowerSchemeSetting, int[]> SettingsSimple
        => new()
        {
            {PowerSchemeSetting.ProcessorThrottle, [5,5,50,60] },
            {PowerSchemeSetting.TurnOffDisplay, [300,7200] },
            {PowerSchemeSetting.Sleep, [1800,10800] },
            {PowerSchemeSetting.WiFi, [3,3] },
            {PowerSchemeSetting.MultimediaPlay, [2,2] },
            {PowerSchemeSetting.MultimediaShare, [0,2] },
            {PowerSchemeSetting.MultimediaQuality, [0,0] }
        };

    private static Dictionary<PowerSchemeSetting, int[]> SettingsExtreme
        => new()
        {
            {PowerSchemeSetting.ProcessorThrottle, [5,100,60,100] },
            {PowerSchemeSetting.TurnOffDisplay, [300,7200] },
            {PowerSchemeSetting.Sleep, [900,10800] },
            {PowerSchemeSetting.WiFi, [0,0] },
            {PowerSchemeSetting.MultimediaPlay, [0,0] },
            {PowerSchemeSetting.MultimediaShare, [2,2] },
            {PowerSchemeSetting.MultimediaQuality, [1,1] }
        };
}

public class Values();

public class StateValues(PowerSchemeDcAcValues state): Values
{
    public PowerSchemeDcAcValues State { get; } = state;
}

public class RangeValues(
    PowerSchemeDcAcValues minState,
    PowerSchemeDcAcValues maxState) : Values
{
    public PowerSchemeDcAcValues MinState { get; } = minState;

    public PowerSchemeDcAcValues MaxState { get; } = maxState;
}

public enum PowerSchemeSetting
{
    ProcessorThrottle,
    Sleep,
    TurnOffDisplay,
    WiFi,
    MultimediaPlay,
    MultimediaShare,
    MultimediaQuality
}
