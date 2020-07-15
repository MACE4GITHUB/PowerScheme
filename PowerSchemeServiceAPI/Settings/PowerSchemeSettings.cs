using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static PowerSchemeServiceAPI.SettingSchemeLookup;

namespace PowerSchemeServiceAPI.Settings
{
    internal class PowerSchemeSettings
    {
        private readonly SettingScheme _settingScheme;

        public PowerSchemeSettings(SettingScheme settingScheme)
        {
            PowerSchemeGuid = SettingSchemes
                .Where(p=>p.Key == settingScheme)
                .Select(p=>p.Value.Guid).FirstOrDefault();

            _settingScheme = settingScheme;
        }

        public Guid PowerSchemeGuid { get; }

        public PowerSchemeProcessorThrottle ProcessorThrottle(Values values)
            => new PowerSchemeProcessorThrottle(PowerSchemeGuid, values.MinState, values.MaxState);

        public PowerSchemeSleep Sleep(Values values)
            => new PowerSchemeSleep(PowerSchemeGuid, values.State);

        public PowerSchemeTurnOffDisplay TurnOffDisplay(Values values)
            => new PowerSchemeTurnOffDisplay(PowerSchemeGuid, values.State);

        public PowerSchemeWiFi WiFi(Values values)
            => new PowerSchemeWiFi(PowerSchemeGuid, values.State);

        public PowerSchemeMultimediaPlay MultimediaPlay(Values values)
            => new PowerSchemeMultimediaPlay(PowerSchemeGuid, values.State);

        public PowerSchemeMultimediaShare MultimediaShare(Values values)
            => new PowerSchemeMultimediaShare(PowerSchemeGuid, values.State);

        public PowerSchemeMultimediaQuality MultimediaQuality(Values values)
            => new PowerSchemeMultimediaQuality(PowerSchemeGuid, values.State);

        public void ApplyDefaultValues()
        {
            var powerSchemeDefaultSettings = new PowerSchemeDefaultSettings(_settingScheme);
            var ds = powerSchemeDefaultSettings.Settings;

            var typeSetting = typeof(PowerSchemeSetting);
            foreach (var name in Enum.GetNames(typeSetting))
            {
                var set = (PowerSchemeSetting)Enum.Parse(typeSetting, name);
                var mi = typeof(PowerSchemeSettings).GetMethod(name, BindingFlags.Instance | BindingFlags.Public);
                var res = mi?.Invoke(this, new object[] { ds[set] });
                var applicable = res as IApplicable;
                applicable?.ApplyValues();
            }
        }
    }

    internal class PowerSchemeDefaultSettings
    {
        public PowerSchemeDefaultSettings(SettingScheme settingScheme)
        {
            Dictionary<PowerSchemeSetting, int[]> defaultSetting;

            switch (settingScheme)
            {
                case SettingScheme.Stable:
                    defaultSetting = SettingsStable;
                    break;
                case SettingScheme.Media:
                    defaultSetting = SettingsMedia;
                    break;
                case SettingScheme.Simple:
                    defaultSetting = SettingsSimple;
                    break;
                case SettingScheme.Extreme:
                    defaultSetting = SettingsExtreme;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(settingScheme), settingScheme, null);
            }

            Settings = new Dictionary<PowerSchemeSetting, Values>
            {
                {PowerSchemeSetting.ProcessorThrottle, new Values( new PowerSchemeDCACValues(defaultSetting[PowerSchemeSetting.ProcessorThrottle][0], defaultSetting[PowerSchemeSetting.ProcessorThrottle][1]),
                                                                   new PowerSchemeDCACValues(defaultSetting[PowerSchemeSetting.ProcessorThrottle][2], defaultSetting[PowerSchemeSetting.ProcessorThrottle][3]))},
                {PowerSchemeSetting.TurnOffDisplay, new Values( new PowerSchemeDCACValues(defaultSetting[PowerSchemeSetting.TurnOffDisplay][0], defaultSetting[PowerSchemeSetting.TurnOffDisplay][1]))},
                {PowerSchemeSetting.Sleep, new Values( new PowerSchemeDCACValues(defaultSetting[PowerSchemeSetting.Sleep][0], defaultSetting[PowerSchemeSetting.Sleep][1]))},
                {PowerSchemeSetting.WiFi, new Values( new PowerSchemeDCACValues(defaultSetting[PowerSchemeSetting.WiFi][0], defaultSetting[PowerSchemeSetting.WiFi][1]))},
                {PowerSchemeSetting.MultimediaPlay, new Values( new PowerSchemeDCACValues(defaultSetting[PowerSchemeSetting.MultimediaPlay][0], defaultSetting[PowerSchemeSetting.MultimediaPlay][1]))},
                {PowerSchemeSetting.MultimediaShare, new Values( new PowerSchemeDCACValues(defaultSetting[PowerSchemeSetting.MultimediaShare][0], defaultSetting[PowerSchemeSetting.MultimediaShare][1]))},
                {PowerSchemeSetting.MultimediaQuality, new Values( new PowerSchemeDCACValues(defaultSetting[PowerSchemeSetting.MultimediaQuality][0], defaultSetting[PowerSchemeSetting.MultimediaQuality][1]))}
            };
        }

        public Dictionary<PowerSchemeSetting, Values> Settings { get; }

        private Dictionary<PowerSchemeSetting, int[]> SettingsStable
            => new Dictionary<PowerSchemeSetting, int[]>
            {
                {PowerSchemeSetting.ProcessorThrottle, new []{5,85,60,85}},
                {PowerSchemeSetting.TurnOffDisplay, new []{300,7200}},
                {PowerSchemeSetting.Sleep, new []{900,10800}},
                {PowerSchemeSetting.WiFi, new []{2,2}},
                {PowerSchemeSetting.MultimediaPlay, new []{1,1}},
                {PowerSchemeSetting.MultimediaShare, new []{2,2}},
                {PowerSchemeSetting.MultimediaQuality, new []{1,1}}
            };

        private Dictionary<PowerSchemeSetting, int[]> SettingsMedia
            => new Dictionary<PowerSchemeSetting, int[]>
            {
                {PowerSchemeSetting.ProcessorThrottle, new []{5,5,60,85}},
                {PowerSchemeSetting.TurnOffDisplay, new []{300,7200}},
                {PowerSchemeSetting.Sleep, new []{1800,10800}},
                {PowerSchemeSetting.WiFi, new []{2,3}},
                {PowerSchemeSetting.MultimediaPlay, new []{2,1}},
                {PowerSchemeSetting.MultimediaShare, new []{0,1}},
                {PowerSchemeSetting.MultimediaQuality, new []{1,1}}
            };

        private Dictionary<PowerSchemeSetting, int[]> SettingsSimple
            => new Dictionary<PowerSchemeSetting, int[]>
            {
                {PowerSchemeSetting.ProcessorThrottle, new []{5,5,50,60}},
                {PowerSchemeSetting.TurnOffDisplay, new []{300,7200}},
                {PowerSchemeSetting.Sleep, new []{1800,10800}},
                {PowerSchemeSetting.WiFi, new []{3,3}},
                {PowerSchemeSetting.MultimediaPlay, new []{2,2}},
                {PowerSchemeSetting.MultimediaShare, new []{0,2}},
                {PowerSchemeSetting.MultimediaQuality, new []{0,0}}
            };

        private Dictionary<PowerSchemeSetting, int[]> SettingsExtreme
            => new Dictionary<PowerSchemeSetting, int[]>
            {
                {PowerSchemeSetting.ProcessorThrottle, new []{5,100,60,100}},
                {PowerSchemeSetting.TurnOffDisplay, new []{300,7200}},
                {PowerSchemeSetting.Sleep, new []{900,10800}},
                {PowerSchemeSetting.WiFi, new []{0,0}},
                {PowerSchemeSetting.MultimediaPlay, new []{0,0}},
                {PowerSchemeSetting.MultimediaShare, new []{2,2}},
                {PowerSchemeSetting.MultimediaQuality, new []{1,1}}
            };
    }

    public class Values
    {

        public Values(PowerSchemeDCACValues state)
        {
            State = state;
        }

        public Values(PowerSchemeDCACValues minState, PowerSchemeDCACValues maxState)
        {
            MinState = minState;
            MaxState = maxState;
        }

        public PowerSchemeDCACValues State { get; }

        public PowerSchemeDCACValues MinState { get; }

        public PowerSchemeDCACValues MaxState { get; }
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
}
