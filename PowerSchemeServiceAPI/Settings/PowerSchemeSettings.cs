using System;
using System.Collections.Generic;
using System.Reflection;

namespace PowerSchemeServiceAPI.Settings
{
    public class PowerSchemeSettings
    {
        public PowerSchemeSettings(Guid guid)
        {
            PowerSchemeGuid = guid;
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
            var powerSchemeDefaultSettings = new PowerSchemeDefaultSettings();
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

    public class PowerSchemeDefaultSettings
    {
        public PowerSchemeDefaultSettings()
        {


            Settings = new Dictionary<PowerSchemeSetting, Values>
            {
                {PowerSchemeSetting.ProcessorThrottle, new Values( new PowerSchemeDCACValues(5, 85),
                                                                   new PowerSchemeDCACValues(60, 85))},
                {PowerSchemeSetting.TurnOffDisplay, new Values( new PowerSchemeDCACValues(300, 7200))},
                {PowerSchemeSetting.Sleep, new Values( new PowerSchemeDCACValues(900, 10800))},
                {PowerSchemeSetting.WiFi, new Values( new PowerSchemeDCACValues(2, 2))},
                {PowerSchemeSetting.MultimediaPlay, new Values( new PowerSchemeDCACValues(1, 1))},
                {PowerSchemeSetting.MultimediaShare, new Values( new PowerSchemeDCACValues(2, 2))},
                {PowerSchemeSetting.MultimediaQuality, new Values( new PowerSchemeDCACValues(1, 1))}
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
