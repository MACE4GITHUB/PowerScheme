using System;
using System.Collections.Generic;
using System.ComponentModel;
using Languages;
using PowerScheme.Configuration;
using PowerSchemeServiceAPI;
using PowerSchemeServiceAPI.Settings;
using static PowerScheme.Addins.IdleMonitoring.Constants;

namespace PowerScheme.Model.Options;

internal class SleepOptions : INotifyPropertyChanged
{
    private readonly IPowerSchemeService _power = DiRoot.GetService<IPowerSchemeService>();

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler? PowerSchemeIdChanged;

    public SleepOptions(
        int sleepInSc = DEFAULT_SLEEP_IN_SECONDS,
        int hibernateInSc = DEFAULT_HIBERNATE_IN_SECONDS)
    {
        SleepIdle = sleepInSc;
        HibernateIdle = hibernateInSc;

        PowerSchemeId = ActivePowerSchemeId;
    }

    public Dictionary<Guid, string> PowerSchemes
    {
        get
        {
            var powerSchemes = new Dictionary<Guid, string>
            {
                { Guid.Empty, Language.Current.AllPowerSchemes }
            };

            foreach (var powerScheme in _power.PowerSchemes)
            {
                powerSchemes.Add(powerScheme.Guid, powerScheme.Name);
            }

            return powerSchemes;
        }
    }

    public Guid PowerSchemeId
    {
        get => field;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;

            SleepIdle = SleepDcAcValues.AcSettings;
            HibernateIdle = HibernateDcAcValues.AcSettings;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PowerSchemeId)));
            PowerSchemeIdChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public Guid DefaultPowerSchemeId => Guid.Empty;

    public Guid ActivePowerSchemeId => _power.ActivePowerScheme.Guid;

    public int SleepIdle { get; set; }

    public string SleepIdleFormated => OptionsHelper.FormatHoursMinutesSeconds(TimeSpan.FromSeconds(SleepIdle));

    public int HibernateIdle { get; set; }

    public string HibernateIdleFormated => OptionsHelper.FormatHoursMinutesSeconds(TimeSpan.FromSeconds(HibernateIdle));

    public bool AcEqualsDc =>
        SleepDcAcValues.AcSettings == SleepDcAcValues.DcSettings &&
        HibernateDcAcValues.AcSettings == HibernateDcAcValues.DcSettings;

    public PowerSchemeDcAcValues SleepDcAcValues =>
        PowerSchemeId == Guid.Empty
            ? _power.GetIdleSleep(ActivePowerSchemeId)
            : _power.GetIdleSleep(PowerSchemeId);

    public void ApplySleepDcAcValues()
    {
        if (PowerSchemeId == Guid.Empty)
        {
            _power.SetAllPowerSchemesIdleSleep(SleepIdle);

            return;
        }

        _power.SetIdleSleep(PowerSchemeId, SleepIdle);
    }

    public PowerSchemeDcAcValues HibernateDcAcValues =>
        PowerSchemeId == Guid.Empty
            ? _power.GetIdleHibernate(ActivePowerSchemeId)
            : _power.GetIdleHibernate(PowerSchemeId);

    public void ApplyHibernateDcAcValues()
    {
        if (PowerSchemeId == Guid.Empty)
        {
            _power.SetAllPowerSchemesIdleHibernate(HibernateIdle);

            return;
        }

        _power.SetIdleHibernate(PowerSchemeId, HibernateIdle);
    }

    public void ApplyDcAcValues()
    {
        ApplySleepDcAcValues();
        ApplyHibernateDcAcValues();
    }
}
