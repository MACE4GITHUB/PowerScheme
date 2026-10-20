using System;
using System.Collections.Generic;
using System.ComponentModel;
using Languages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerScheme.Model.Options;
using PowerScheme.Tests.TestDoubles;
using PowerSchemeServiceAPI.Settings;

namespace PowerScheme.Tests;

[TestClass]
[DoNotParallelize]
public sealed class PowerSchemeOptionsTests
{
    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void Constructor_SelectsActiveSchemeAndLoadsAcValues(bool display)
    {
        var context = new TestContext(display);

        Assert.AreEqual(context.Main.Guid, context.Options.PowerSchemeId);
        Assert.AreEqual(context.Main.Guid, context.Options.ActivePowerSchemeId);
        Assert.AreEqual((11, 21), context.EditedValues);
        Assert.AreEqual(2, context.Power.Reads.Count);
        Assert.AreEqual(0, context.Power.Writes.Count);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void Constructor_EmptyActiveId_PreservesProvidedDefaults(bool display)
    {
        var power = new TestPowerService(new TestScheme(Guid.Empty));
        if (display)
        {
            var options = new DisplayOptions(power, 91, 92);
            Assert.AreEqual(91, options.TurnOffDisplay);
            Assert.AreEqual(92, options.TurnOffLockDisplay);
        }
        else
        {
            var options = new SleepOptions(power, 91, 92);
            Assert.AreEqual(91, options.SleepIdle);
            Assert.AreEqual(92, options.HibernateIdle);
        }

        Assert.AreEqual(0, power.Reads.Count);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void SelectionChange_LoadsValuesBeforeRaisingNotificationsInOrder(bool display)
    {
        var context = new TestContext(display);
        var notifications = new List<string>();
        context.Options.PropertyChanged += (sender, e) =>
        {
            Assert.AreSame(context.Options, sender);
            Assert.AreEqual(nameof(PowerSchemeOptions.PowerSchemeId), e.PropertyName);
            Assert.AreEqual((31, 41), context.EditedValues);
            notifications.Add("property");
        };
        context.Options.PowerSchemeIdChanged += (sender, _) =>
        {
            Assert.AreSame(context.Options, sender);
            Assert.AreEqual((31, 41), context.EditedValues);
            notifications.Add("scheme");
        };

        context.Options.PowerSchemeId = context.Other.Guid;

        CollectionAssert.AreEqual(new[] { "property", "scheme" }, notifications);
        Assert.AreEqual(context.Other.Guid, context.Options.PowerSchemeId);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void SameSelection_DoesNotReloadValuesOrRaiseNotifications(bool display)
    {
        var context = new TestContext(display);
        context.SetEditedValues(71, 72);
        context.Power.Reads.Clear();
        var notificationCount = 0;
        context.Options.PropertyChanged += (_, _) => notificationCount++;
        context.Options.PowerSchemeIdChanged += (_, _) => notificationCount++;

        context.Options.PowerSchemeId = context.Main.Guid;

        Assert.AreEqual((71, 72), context.EditedValues);
        Assert.AreEqual(0, context.Power.Reads.Count);
        Assert.AreEqual(0, notificationCount);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void AllSchemeSelection_ReadsCurrentActiveSchemeWithoutCaching(bool display)
    {
        var context = new TestContext(display);
        context.Options.PowerSchemeId = context.Options.DefaultPowerSchemeId;
        Assert.AreEqual(Guid.Empty, context.Options.PowerSchemeId);
        Assert.AreEqual((11, 21), context.EditedValues);

        context.Power.SetActivePowerScheme(context.Other);

        Assert.AreEqual(context.Other.Guid, context.Options.ActivePowerSchemeId);
        Assert.AreEqual(31, context.FirstValues.AcSettings);
        Assert.AreEqual(41, context.SecondValues.AcSettings);
        Assert.AreEqual(Guid.Empty, context.Options.PowerSchemeId);
    }

    [TestMethod]
    [DataRow(true, false)]
    [DataRow(true, true)]
    [DataRow(false, false)]
    [DataRow(false, true)]
    public void ApplyValues_RoutesBothSettingsToSelectedOrAllSchemes(bool display, bool allSchemes)
    {
        var context = new TestContext(display);
        var selectedId = allSchemes ? Guid.Empty : context.Other.Guid;
        context.Options.PowerSchemeId = selectedId;
        context.SetEditedValues(71, 72);

        context.ApplyValues();

        Guid? expectedTarget = allSchemes ? null : selectedId;
        Assert.AreEqual(2, context.Power.Writes.Count);
        Assert.AreEqual((context.FirstSetting, expectedTarget, 71), context.Power.Writes[0]);
        Assert.AreEqual((context.SecondSetting, expectedTarget, 72), context.Power.Writes[1]);
    }

    [TestMethod]
    [DataRow(true, true, true)]
    [DataRow(true, false, true)]
    [DataRow(true, true, false)]
    [DataRow(true, false, false)]
    [DataRow(false, true, true)]
    [DataRow(false, false, true)]
    [DataRow(false, true, false)]
    [DataRow(false, false, false)]
    public void AcEqualsDc_RequiresBothSettingsToMatch(bool display, bool firstMatches, bool secondMatches)
    {
        var context = new TestContext(display);
        context.Power.Values[(context.FirstSetting, context.Main.Guid)] = new PowerSchemeDcAcValues(10, firstMatches ? 10 : 11);
        context.Power.Values[(context.SecondSetting, context.Main.Guid)] = new PowerSchemeDcAcValues(20, secondMatches ? 20 : 21);

        Assert.AreEqual(firstMatches && secondMatches, context.AcEqualsDc);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void PowerSchemes_RefreshesLanguageNamesAndSchemeListOnEveryRead(bool display)
    {
        var context = new TestContext(display);
        var originalLanguage = Language.Kind;
        try
        {
            Language.SetLanguage(LanguageKind.En);
            var english = context.Options.PowerSchemes;
            Assert.AreEqual(Language.Current.AllPowerSchemes, english[Guid.Empty]);
            Assert.AreEqual(3, english.Count);
            Assert.AreEqual("Main", english[context.Main.Guid]);

            Language.SetLanguage(LanguageKind.Ru);
            context.Main.Name = "Renamed";
            context.Power.Schemes = [context.Main];
            var russian = context.Options.PowerSchemes;

            Assert.AreNotSame(english, russian);
            Assert.AreEqual(Language.Current.AllPowerSchemes, russian[Guid.Empty]);
            Assert.AreNotEqual(english[Guid.Empty], russian[Guid.Empty]);
            Assert.AreEqual("Renamed", russian[context.Main.Guid]);
            Assert.AreEqual(2, russian.Count);
            Assert.IsFalse(russian.ContainsKey(context.Other.Guid));
        }
        finally
        {
            Language.SetLanguage(originalLanguage);
        }
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void FormattedValues_UseExistingTimeFormatter(bool display)
    {
        var context = new TestContext(display);
        context.SetEditedValues(3_661, 61);

        var expectedFirst = OptionsHelper.FormatHoursMinutesSeconds(TimeSpan.FromSeconds(3_661));
        var expectedSecond = OptionsHelper.FormatHoursMinutesSeconds(TimeSpan.FromSeconds(61));
        if (context.Options is DisplayOptions displayOptions)
        {
            Assert.AreEqual(expectedFirst, displayOptions.TurnOffDisplayFormated);
            Assert.AreEqual(expectedSecond, displayOptions.TurnOffLockedDisplayFormated);
        }
        else
        {
            var sleepOptions = (SleepOptions)context.Options;
            Assert.AreEqual(expectedFirst, sleepOptions.SleepIdleFormated);
            Assert.AreEqual(expectedSecond, sleepOptions.HibernateIdleFormated);
        }
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void BindingProperties_RemainDiscoverableAfterMovingToBaseClass(bool display)
    {
        var context = new TestContext(display);
        var properties = TypeDescriptor.GetProperties(context.Options);
        var names = new[]
        {
            nameof(PowerSchemeOptions.PowerSchemeId),
            nameof(PowerSchemeOptions.PowerSchemes),
            nameof(PowerSchemeOptions.ActivePowerSchemeId),
            nameof(PowerSchemeOptions.DefaultPowerSchemeId),
            display ? nameof(DisplayOptions.TurnOffDisplay) : nameof(SleepOptions.SleepIdle),
            display ? nameof(DisplayOptions.TurnOffLockDisplay) : nameof(SleepOptions.HibernateIdle)
        };

        foreach (var name in names)
        {
            Assert.IsNotNull(properties[name], $"Missing binding property: {name}");
        }
    }

    private sealed class TestContext
    {
        public TestScheme Main { get; } = new(Guid.NewGuid()) { Name = "Main" };
        public TestScheme Other { get; } = new(Guid.NewGuid()) { Name = "Other" };
        public TestPowerService Power { get; }
        public PowerSchemeOptions Options { get; }
        public string FirstSetting { get; }
        public string SecondSetting { get; }

        public TestContext(bool display)
        {
            FirstSetting = display ? "display" : "sleep";
            SecondSetting = display ? "lock" : "hibernate";
            Power = new TestPowerService(Main) { Schemes = [Main, Other] };
            Power.Values[(FirstSetting, Main.Guid)] = new PowerSchemeDcAcValues(10, 11);
            Power.Values[(SecondSetting, Main.Guid)] = new PowerSchemeDcAcValues(20, 21);
            Power.Values[(FirstSetting, Other.Guid)] = new PowerSchemeDcAcValues(30, 31);
            Power.Values[(SecondSetting, Other.Guid)] = new PowerSchemeDcAcValues(40, 41);
            Options = display ? new DisplayOptions(Power) : new SleepOptions(Power);
        }

        public (int First, int Second) EditedValues => Options is DisplayOptions display
            ? (display.TurnOffDisplay, display.TurnOffLockDisplay)
            : (((SleepOptions)Options).SleepIdle, ((SleepOptions)Options).HibernateIdle);

        public PowerSchemeDcAcValues FirstValues => Options is DisplayOptions display
            ? display.TurnOffDisplayDcAcValues
            : ((SleepOptions)Options).SleepDcAcValues;

        public PowerSchemeDcAcValues SecondValues => Options is DisplayOptions display
            ? display.TurnOffLockDisplayDcAcValues
            : ((SleepOptions)Options).HibernateDcAcValues;

        public bool AcEqualsDc => Options is DisplayOptions display ? display.AcEqualsDc : ((SleepOptions)Options).AcEqualsDc;

        public void SetEditedValues(int first, int second)
        {
            if (Options is DisplayOptions display)
            {
                display.TurnOffDisplay = first;
                display.TurnOffLockDisplay = second;
            }
            else
            {
                var sleep = (SleepOptions)Options;
                sleep.SleepIdle = first;
                sleep.HibernateIdle = second;
            }
        }

        public void ApplyValues()
        {
            if (Options is DisplayOptions display)
            {
                display.ApplyDcAcValues();
            }
            else
            {
                ((SleepOptions)Options).ApplyDcAcValues();
            }
        }
    }
}
