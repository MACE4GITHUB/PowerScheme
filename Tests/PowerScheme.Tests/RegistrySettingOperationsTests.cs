using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using RegistryManager;
using RegistryManager.Common;

namespace PowerScheme.Tests;

[TestClass]
public sealed class RegistrySettingOperationsTests
{
    [TestMethod]
    [DataRow(0, null)]
    [DataRow(1, null)]
    [DataRow(99, null)]
    [DataRow("EN", "en")]
    [DataRow("ru", "ru")]
    [DataRow(" DE-de ", "de-de")]
    [DataRow("", null)]
    [DataRow("   ", null)]
    [DataRow(null, null)]
    public void GetLanguageCode_ReadsOnlyStringsWithoutChangingStoredValue(object? value, string? expected)
    {
        var company = "PowerScheme.Tests." + Guid.NewGuid().ToString("N");
        const string product = "LanguageSelection";
        var companyPath = @"SOFTWARE\" + company;
        try
        {
            using var key = Registry.CurrentUser.CreateSubKey(companyPath + @"\" + product);
            if (value != null)
            {
                key.SetValue("LanguageId", value);
            }

            var actual = RegistryService.GetLanguageCode(company, product);

            Assert.AreEqual(expected, actual);
            Assert.AreEqual(value, key.GetValue("LanguageId"));
        }
        finally
        {
            Registry.CurrentUser.DeleteSubKeyTree(companyPath, false);
        }
    }

    [TestMethod]
    [DataRow("en", "en")]
    [DataRow(" EN-us ", "en-us")]
    [DataRow("0", "0")]
    [DataRow("1", "1")]
    [DataRow(" DE-de ", "de-de")]
    [DataRow("ru", "ru")]
    [DataRow("", null)]
    [DataRow(null, null)]
    [DataRow("fr", "fr")]
    public void NormalizeLanguageCode_UsesSameStringRulesForEveryLanguage(string? value, string? expected) =>
        Assert.AreEqual(expected, RegistryService.NormalizeLanguageCode(value));

    [TestMethod]
    [DataRow(0, RegistryValueKind.DWord)]
    [DataRow(-1, RegistryValueKind.DWord)]
    [DataRow(60_000, RegistryValueKind.DWord)]
    [DataRow("scheme-id", RegistryValueKind.String)]
    [DataRow("", RegistryValueKind.String)]
    public void SetRegistryValue_PreservesMetadataAndSavesAssignedValueOnce(object value, RegistryValueKind expectedKind)
    {
        var parameter = CreateParameter();
        var saveCount = 0;

        RegistryService.SetRegistryValue(parameter, value, saved =>
        {
            saveCount++;
            Assert.AreEqual(parameter.RegistryHive, saved.RegistryHive);
            Assert.AreEqual(parameter.Path, saved.Path);
            Assert.AreEqual(parameter.Section, saved.Section);
            Assert.AreEqual(parameter.Name, saved.Name);
            Assert.AreEqual(parameter.RegistryKey, saved.RegistryKey);
            Assert.AreSame(value, saved.Value);
            Assert.AreEqual(expectedKind, saved.RegistryValueKind);
        });

        Assert.AreEqual(1, saveCount);
    }

    [TestMethod]
    [DataRow(RegistryValueKind.DWord)]
    [DataRow(RegistryValueKind.ExpandString)]
    [DataRow(RegistryValueKind.String)]
    public void SetRegistryValue_PreservesExplicitKindInsteadOfRecalculatingIt(RegistryValueKind kind)
    {
        var parameter = CreateParameter();
        parameter.RegistryValueKind = kind;

        RegistryService.SetRegistryValue(parameter, 61, saved =>
        {
            Assert.AreEqual(kind, saved.RegistryValueKind);
            Assert.AreEqual(61, saved.Value);
        });
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void SetRegistryValue_PreservesArrayReferenceAndInferredKind(bool binary)
    {
        object value = binary ? new byte[] { 1, 2 } : new[] { "first", "second" };

        RegistryService.SetRegistryValue(CreateParameter(), value, saved =>
        {
            Assert.AreSame(value, saved.Value);
            Assert.AreEqual(binary ? RegistryValueKind.Binary : RegistryValueKind.MultiString, saved.RegistryValueKind);
        });
    }

    [TestMethod]
    public void SetRegistryValue_DoesNotMutateOriginalStruct()
    {
        var parameter = CreateParameter();
        parameter.Value = "original";

        RegistryService.SetRegistryValue(parameter, 61, saved => Assert.AreEqual(61, saved.Value));

        Assert.AreEqual("original", parameter.Value);
        Assert.AreEqual(RegistryValueKind.String, parameter.RegistryValueKind);
    }

    [TestMethod]
    public void SetRegistryValue_PropagatesSaverFailureWithoutRetry()
    {
        var expected = new UnauthorizedAccessException("Save failed.");
        var saveCount = 0;

        var actual = Assert.ThrowsExactly<UnauthorizedAccessException>(() =>
            RegistryService.SetRegistryValue(CreateParameter(), 61, saved =>
            {
                saveCount++;
                Assert.AreEqual(61, saved.Value);
                throw expected;
            }));

        Assert.AreSame(expected, actual);
        Assert.AreEqual(1, saveCount);
    }

    [TestMethod]
    public void SetRegistryValue_NullIsPassedToSaverWithoutSubstitution()
    {
        var parameter = CreateParameter();
        parameter.Value = "previous";
        var expected = new ArgumentException("Value");

        var actual = Assert.ThrowsExactly<ArgumentException>(() =>
            RegistryService.SetRegistryValue(parameter, null!, saved =>
            {
                Assert.IsNull(saved.Value);
                Assert.AreEqual(RegistryValueKind.None, saved.RegistryValueKind);
                throw expected;
            }));

        Assert.AreSame(expected, actual);
    }

    private static RegistryParam CreateParameter() => new()
    {
        RegistryHive = RegistryHive.CurrentUser,
        Path = @"SOFTWARE\TestCompany\TestProduct",
        Section = "IdleMonitor",
        Name = "TestValue"
    };
}