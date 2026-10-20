using System;
using System.Windows.Forms;
using PowerScheme.Model.Options;

namespace PowerScheme.Controls;

internal static class TimeSettingBinding
{
    internal static void Bind(
        NumericUpDown input,
        Label formattedValue,
        object options,
        string valueProperty,
        string formattedProperty,
        int minimum,
        int maximum)
    {
        input.DataBindings.Add(nameof(input.Value), options, valueProperty, false, DataSourceUpdateMode.OnPropertyChanged);
        formattedValue.DataBindings.Add(nameof(formattedValue.Text), options, formattedProperty);
        input.KeyUp += (_, _) =>
            formattedValue.Text = OptionsHelper.FormatHoursMinutesSeconds(TimeSpan.FromSeconds((int)input.Value));
        input.Minimum = minimum;
        input.Maximum = maximum;
    }
}