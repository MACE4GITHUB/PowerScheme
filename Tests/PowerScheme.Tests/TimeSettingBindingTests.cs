using System;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerScheme.Controls;
using PowerScheme.Model.Options;
using PowerScheme.Tests.TestDoubles;
using static PowerScheme.Addins.IdleMonitoring.Constants;

namespace PowerScheme.Tests;

[STATestClass]
public sealed class TimeSettingBindingTests
{
    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    public void Bind_ConfiguresExistingPropertiesAndRange(int setting)
    {
        var (options, valueProperty, formattedProperty, minimum, maximum) = CreateSetting(setting);
        using var input = new NumericUpDown();
        using var label = new Label();

        TimeSettingBinding.Bind(input, label, options, valueProperty, formattedProperty, minimum, maximum);

        Assert.AreEqual((decimal)minimum, input.Minimum);
        Assert.AreEqual((decimal)maximum, input.Maximum);
        Assert.AreEqual(1, input.DataBindings.Count);
        Assert.AreEqual(1, label.DataBindings.Count);
        Assert.AreSame(options, input.DataBindings[0].DataSource);
        Assert.AreEqual(nameof(NumericUpDown.Value), input.DataBindings[0].PropertyName);
        Assert.AreEqual(valueProperty, input.DataBindings[0].BindingMemberInfo.BindingMember);
        Assert.AreEqual(DataSourceUpdateMode.OnPropertyChanged, input.DataBindings[0].DataSourceUpdateMode);
        Assert.IsFalse(input.DataBindings[0].FormattingEnabled);
        Assert.AreSame(options, label.DataBindings[0].DataSource);
        Assert.AreEqual(formattedProperty, label.DataBindings[0].BindingMemberInfo.BindingMember);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    public void Binding_ReadsModelAndWritesEditedValue(int setting)
    {
        var (options, valueProperty, formattedProperty, minimum, maximum) = CreateSetting(setting);
        using var parent = new UserControl { BindingContext = new BindingContext() };
        using var input = new NumericUpDown();
        using var label = new Label();
        parent.Controls.Add(input);
        parent.Controls.Add(label);
        TimeSettingBinding.Bind(input, label, options, valueProperty, formattedProperty, minimum, maximum);
        parent.CreateControl();
        var property = TypeDescriptor.GetProperties(options)[valueProperty]!;

        property.SetValue(options, 61);
        input.DataBindings[0].ReadValue();
        label.DataBindings[0].ReadValue();

        Assert.AreEqual(61m, input.Value);
        Assert.AreEqual(Format(61), label.Text);

        input.Value = 3_661;

        Assert.AreEqual(3_661, property.GetValue(options));
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(61)]
    [DataRow(3_661)]
    public void KeyUp_RefreshesFormattedLabelWithoutAVisibleWindow(int seconds)
    {
        var (options, valueProperty, formattedProperty, minimum, maximum) = CreateSetting(0);
        using var input = new TestNumericUpDown();
        using var label = new Label();
        TimeSettingBinding.Bind(input, label, options, valueProperty, formattedProperty, minimum, maximum);
        input.Value = seconds;
        label.Text = "stale";

        input.RaiseKeyUp();

        Assert.AreEqual(Format(seconds), label.Text);
        Assert.IsFalse(input.Visible && input.IsHandleCreated);
    }

    [TestMethod]
    public void SeparateBindings_DoNotUpdateOtherLabels()
    {
        var (options, valueProperty, formattedProperty, minimum, maximum) = CreateSetting(0);
        using var first = new TestNumericUpDown();
        using var second = new TestNumericUpDown();
        using var firstLabel = new Label();
        using var secondLabel = new Label();
        TimeSettingBinding.Bind(first, firstLabel, options, valueProperty, formattedProperty, minimum, maximum);
        TimeSettingBinding.Bind(second, secondLabel, options, valueProperty, formattedProperty, minimum, maximum);
        first.Value = 61;
        secondLabel.Text = "unchanged";

        first.RaiseKeyUp();

        Assert.AreEqual(Format(61), firstLabel.Text);
        Assert.AreEqual("unchanged", secondLabel.Text);
    }

    private static string Format(int seconds) =>
        OptionsHelper.FormatHoursMinutesSeconds(TimeSpan.FromSeconds(seconds));

    private static (PowerSchemeOptions Options, string Value, string Formatted, int Minimum, int Maximum) CreateSetting(int setting)
    {
        var power = new TestPowerService(new TestScheme(Guid.Empty));
        return setting switch
        {
            0 => (new DisplayOptions(power), nameof(DisplayOptions.TurnOffDisplay), nameof(DisplayOptions.TurnOffDisplayFormated),
                MIN_TURN_OFF_DISPLAY_IN_SECONDS, MAX_TURN_OFF_DISPLAY_IN_SECONDS),
            1 => (new DisplayOptions(power), nameof(DisplayOptions.TurnOffLockDisplay), nameof(DisplayOptions.TurnOffLockedDisplayFormated),
                MIN_TURN_OFF_LOCK_DISPLAY_IN_SECONDS, MAX_TURN_OFF_LOCK_DISPLAY_IN_SECONDS),
            2 => (new SleepOptions(power), nameof(SleepOptions.SleepIdle), nameof(SleepOptions.SleepIdleFormated),
                MIN_SLEEP_IN_SECONDS, MAX_SLEEP_IN_SECONDS),
            3 => (new SleepOptions(power), nameof(SleepOptions.HibernateIdle), nameof(SleepOptions.HibernateIdleFormated),
                MIN_HIBERNATE_IN_SECONDS, MAX_HIBERNATE_IN_SECONDS),
            _ => throw new ArgumentOutOfRangeException(nameof(setting))
        };
    }

    private sealed class TestNumericUpDown : NumericUpDown
    {
        public void RaiseKeyUp() => OnKeyUp(new KeyEventArgs(Keys.D1));
    }
}