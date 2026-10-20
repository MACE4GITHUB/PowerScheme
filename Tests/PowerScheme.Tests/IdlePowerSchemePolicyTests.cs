using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerScheme.Services;

namespace PowerScheme.Tests;

[TestClass]
public sealed class IdlePowerSchemePolicyTests
{
    [TestMethod]
    [DataRow(1, 1, 1, 2, 1)]
    [DataRow(1, 1, 1, 1, -1)]
    [DataRow(1, 2, 1, 3, 2)]
    [DataRow(1, 2, 1, 2, -1)]
    [DataRow(2, 3, 1, 2, 3)]
    [DataRow(2, 3, 1, 3, -1)]
    [DataRow(2, 2, 1, 3, 2)]
    [DataRow(2, 2, 1, 2, -1)]
    [DataRow(2, 1, 1, 3, 2)]
    [DataRow(2, 1, 1, 2, -1)]
    [DataRow(2, 1, 1, 1, 2)]
    [DataRow(0, 1, 1, 2, 0)]
    [DataRow(1, 0, 1, 2, 0)]
    [DataRow(3, 2, 1, 0, 2)]
    [DataRow(2, 1, 0, 3, -1)]
    [DataRow(1, 1, 0, 3, -1)]
    [DataRow(2, 3, 0, 3, -1)]
    [DataRow(0, 0, 1, 2, 0)]
    [DataRow(0, 0, 0, 2, -1)]
    public void GetMainPowerSchemeUpdate_ReturnsExpectedDecision(
        int previous,
        int active,
        int idle,
        int main,
        int expected)
    {
        var actual = IdlePowerSchemePolicy.GetMainPowerSchemeUpdate(
            SchemeId(previous), SchemeId(active), SchemeId(idle), SchemeId(main));

        Guid? expectedUpdate = expected == -1 ? null : SchemeId(expected);
        Assert.AreEqual(expectedUpdate, actual);
    }

    [TestMethod]
    public void GetMainPowerSchemeUpdate_EnteringIdle_PreservesPreviousMainScheme()
    {
        var main = SchemeId(2);
        var idle = SchemeId(1);

        var update = IdlePowerSchemePolicy.GetMainPowerSchemeUpdate(main, idle, idle, main);

        Assert.IsNull(update);
    }

    private static Guid SchemeId(int value) => new(value, 0, 0, new byte[8]);
}
