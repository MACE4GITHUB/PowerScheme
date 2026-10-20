using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerManagerAPI;
using PowerSchemeServiceAPI;
using PowerSchemeServiceAPI.Settings;

namespace PowerScheme.Tests;

[TestClass]
public sealed class PowerSettingOperationsTests
{
    [TestMethod]
    [DataRow(SettingSubgroup.VIDEO_SUBGROUP, Setting.VIDEOIDLE)]
    [DataRow(SettingSubgroup.VIDEO_SUBGROUP, Setting.VIDEOCONLOCK)]
    [DataRow(SettingSubgroup.SLEEP_SUBGROUP, Setting.STANDBYIDLE)]
    [DataRow(SettingSubgroup.SLEEP_SUBGROUP, Setting.HIBERNATEIDLE)]
    public void ReadPowerSetting_ForwardsIdsAndReadsDcBeforeAc(SettingSubgroup subgroup, Setting setting)
    {
        var schemeId = Guid.NewGuid();
        var modes = new List<PowerMode>();

        var values = PowerSchemeService.ReadPowerSetting(schemeId, subgroup, setting, (id, actualSubgroup, actualSetting, mode) =>
        {
            Assert.AreEqual(schemeId, id);
            Assert.AreEqual(subgroup, actualSubgroup);
            Assert.AreEqual(setting, actualSetting);
            modes.Add(mode);
            return mode == PowerMode.DC ? 61u : 3_661u;
        });

        CollectionAssert.AreEqual(new[] { PowerMode.DC, PowerMode.AC }, modes);
        Assert.AreEqual(61, values.DcSettings);
        Assert.AreEqual(3_661, values.AcSettings);
    }

    [TestMethod]
    public void ReadPowerSetting_PreservesExistingUnsignedConversion()
    {
        var values = PowerSchemeService.ReadPowerSetting(Guid.NewGuid(), SettingSubgroup.VIDEO_SUBGROUP, Setting.VIDEOIDLE,
            (_, _, _, _) => uint.MaxValue);

        Assert.AreEqual(-1, values.DcSettings);
        Assert.AreEqual(-1, values.AcSettings);
    }

    [TestMethod]
    [DataRow(PowerMode.DC)]
    [DataRow(PowerMode.AC)]
    public void ReadPowerSetting_PropagatesFailureAndDoesNotContinue(PowerMode failingMode)
    {
        var expected = new InvalidOperationException("Read failed.");
        var modes = new List<PowerMode>();

        var actual = Assert.ThrowsExactly<InvalidOperationException>(() =>
            PowerSchemeService.ReadPowerSetting(Guid.NewGuid(), SettingSubgroup.VIDEO_SUBGROUP, Setting.VIDEOIDLE,
                (_, _, _, mode) =>
                {
                    modes.Add(mode);
                    if (mode == failingMode)
                    {
                        throw expected;
                    }

                    return 1u;
                }));

        Assert.AreSame(expected, actual);
        Assert.AreEqual(failingMode == PowerMode.DC ? 1 : 2, modes.Count);
    }

    [TestMethod]
    [DataRow(-2)]
    [DataRow(-1)]
    [DataRow(0)]
    [DataRow(61)]
    [DataRow(3_661)]
    public void ApplyPowerSettings_PassesUnchangedValueToBothModesAndAppliesEachScheme(int value)
    {
        var schemeIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var appliedIds = new List<Guid>();
        var suppliedValues = new List<PowerSchemeDcAcValues>();

        PowerSchemeService.ApplyPowerSettings(schemeIds, value, (id, values) =>
        {
            suppliedValues.Add(values);
            Assert.AreEqual(value, values.DcSettings);
            Assert.AreEqual(value, values.AcSettings);
            return new TestSetting(() => appliedIds.Add(id));
        });

        CollectionAssert.AreEqual(schemeIds, appliedIds);
        Assert.AreSame(suppliedValues[0], suppliedValues[1]);
    }

    [TestMethod]
    public void ApplyPowerSettings_EmptySequence_DoesNotCreateSettings()
    {
        PowerSchemeService.ApplyPowerSettings([], 61, (_, _) =>
        {
            Assert.Fail("No setting should be created for an empty sequence.");
            return null!;
        });
    }

    [TestMethod]
    public void ApplyPowerSettings_AppliesBeforeRequestingNextSchemeWithoutReenumerating()
    {
        var calls = new List<string>();
        IEnumerable<Guid> GetIds()
        {
            calls.Add("first");
            yield return Guid.NewGuid();
            calls.Add("second");
            yield return Guid.NewGuid();
        }

        PowerSchemeService.ApplyPowerSettings(GetIds(), 61, (_, _) =>
        {
            calls.Add("create");
            return new TestSetting(() => calls.Add("apply"));
        });

        CollectionAssert.AreEqual(new[] { "first", "create", "apply", "second", "create", "apply" }, calls);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void ApplyPowerSettings_FailureStopsBeforeNextScheme(bool factoryFails)
    {
        var expected = new InvalidOperationException("Apply failed.");
        var requestedIds = 0;
        IEnumerable<Guid> GetIds()
        {
            requestedIds++;
            yield return Guid.NewGuid();
            requestedIds++;
            yield return Guid.NewGuid();
        }

        var actual = Assert.ThrowsExactly<InvalidOperationException>(() =>
            PowerSchemeService.ApplyPowerSettings(GetIds(), 61, (_, _) =>
            {
                if (factoryFails)
                {
                    throw expected;
                }

                return new TestSetting(() => throw expected);
            }));

        Assert.AreSame(expected, actual);
        Assert.AreEqual(1, requestedIds);
    }

    private sealed class TestSetting(Action apply) : IApplicable
    {
        public void ApplyValues() => apply();
    }
}