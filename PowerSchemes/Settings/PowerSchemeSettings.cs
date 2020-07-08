using System;
using System.Collections.Generic;
using System.Reflection;

namespace PowerSchemes.Settings
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

        public void ApplyDefaultValues()
        {
            var powerSchemeDefaultSettings = new PowerSchemeDefaultSettings();
            var ds = powerSchemeDefaultSettings.Settings;

            var typeSetting = typeof(PowerSchemeSetting);
            foreach (var name in Enum.GetNames(typeSetting))
            {
                var set = (PowerSchemeSetting)Enum.Parse(typeSetting, name);
                var mi = typeof(PowerSchemeSettings).GetMethod(name, BindingFlags.Instance | BindingFlags.Public);
                var res = mi?.Invoke(this, new[] { ds[set] });
                var applicable = res as IApplicable;
                applicable?.ApplyValues();
            }
        }
    }

    public class PowerSchemeDefaultSettings
    {
        public PowerSchemeDefaultSettings()
        {
            Settings = new Dictionary<PowerSchemeSetting, Values>()
            {
                {PowerSchemeSetting.ProcessorThrottle,
                    new Values( new PowerSchemeDCACValues(5, 85),
                                new PowerSchemeDCACValues(60, 85))},
                {PowerSchemeSetting.Sleep,
                    new Values( new PowerSchemeDCACValues(900, 10800))},
                {PowerSchemeSetting.TurnOffDisplay,
                    new Values( new PowerSchemeDCACValues(300, 7200))}
            };
        }

        public Dictionary<PowerSchemeSetting, Values> Settings { get; }
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
        TurnOffDisplay
    }
}
