using System;
using System.Collections.Generic;
using System.ComponentModel;
using Languages;
using PowerSchemeServiceAPI;

namespace PowerScheme.Model.Options;

internal abstract class PowerSchemeOptions(IPowerSchemeService power) : INotifyPropertyChanged
{
    protected IPowerSchemeService Power { get; } = power;

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler? PowerSchemeIdChanged;

    public Dictionary<Guid, string> PowerSchemes
    {
        get
        {
            var powerSchemes = new Dictionary<Guid, string>
            {
                { Guid.Empty, Language.Current.AllPowerSchemes }
            };

            foreach (var powerScheme in Power.PowerSchemes)
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
            LoadSelectedValues();

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PowerSchemeId)));
            PowerSchemeIdChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public Guid DefaultPowerSchemeId => Guid.Empty;

    public Guid ActivePowerSchemeId => Power.ActivePowerScheme.Guid;

    protected Guid EffectivePowerSchemeId =>
        PowerSchemeId == Guid.Empty ? ActivePowerSchemeId : PowerSchemeId;

    protected void ApplyValue(int value, Action<Guid, int> apply, Action<int> applyAll)
    {
        if (PowerSchemeId == Guid.Empty)
        {
            applyAll(value);
            return;
        }

        apply(PowerSchemeId, value);
    }

    protected abstract void LoadSelectedValues();
}
