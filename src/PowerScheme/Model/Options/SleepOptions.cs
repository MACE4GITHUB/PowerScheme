using System;
using PowerScheme.Configuration;
using PowerSchemeServiceAPI;
using PowerSchemeServiceAPI.Settings;
using static PowerScheme.Addins.IdleMonitoring.Constants;

namespace PowerScheme.Model.Options;

internal class SleepOptions : PowerSchemeOptions
{
    public SleepOptions(
        int sleepInSc = DEFAULT_SLEEP_IN_SECONDS,
        int hibernateInSc = DEFAULT_HIBERNATE_IN_SECONDS)
        : this(DiRoot.GetService<IPowerSchemeService>(), sleepInSc, hibernateInSc)
    {
    }

    internal SleepOptions(
        IPowerSchemeService power,
        int sleepInSc = DEFAULT_SLEEP_IN_SECONDS,
        int hibernateInSc = DEFAULT_HIBERNATE_IN_SECONDS)
        : base(power)
    {
        SleepIdle = sleepInSc;
        HibernateIdle = hibernateInSc;

        PowerSchemeId = ActivePowerSchemeId;
    }

    public int SleepIdle { get; set; }

    public string SleepIdleFormated => OptionsHelper.FormatHoursMinutesSeconds(TimeSpan.FromSeconds(SleepIdle));

    public int HibernateIdle { get; set; }

    public string HibernateIdleFormated => OptionsHelper.FormatHoursMinutesSeconds(TimeSpan.FromSeconds(HibernateIdle));

    public bool AcEqualsDc =>
        SleepDcAcValues.AcSettings == SleepDcAcValues.DcSettings &&
        HibernateDcAcValues.AcSettings == HibernateDcAcValues.DcSettings;

    public PowerSchemeDcAcValues SleepDcAcValues =>
        Power.GetIdleSleep(EffectivePowerSchemeId);

    public void ApplySleepDcAcValues() =>
        ApplyValue(SleepIdle, Power.SetIdleSleep, Power.SetAllPowerSchemesIdleSleep);

    public PowerSchemeDcAcValues HibernateDcAcValues =>
        Power.GetIdleHibernate(EffectivePowerSchemeId);

    public void ApplyHibernateDcAcValues() =>
        ApplyValue(HibernateIdle, Power.SetIdleHibernate, Power.SetAllPowerSchemesIdleHibernate);

    public void ApplyDcAcValues()
    {
        ApplySleepDcAcValues();
        ApplyHibernateDcAcValues();
    }

    protected override void LoadSelectedValues()
    {
        SleepIdle = SleepDcAcValues.AcSettings;
        HibernateIdle = HibernateDcAcValues.AcSettings;
    }
}
