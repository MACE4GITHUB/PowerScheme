using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Common;
using Languages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerScheme.Model.Menu.Settings;
using PowerScheme.Tests.TestDoubles;
using PowerScheme.Utility;

namespace PowerScheme.Tests;

[STATestClass]
[DoNotParallelize]
public sealed class LanguageResourcesTests
{
    [TestMethod]
    [DataRow("de-DE", "de")]
    [DataRow("de-AT", "de-at")]
    [DataRow("ru-RU", "ru")]
    [DataRow("be-BY", "ru")]
    [DataRow("en-US", "en")]
    [DataRow("fr-FR", "en")]
    public void GetLanguage_MatchesDiscoveredCultureWithFallback(string locale, string expected)
    {
        var directory = Path.Combine(Path.GetTempPath(), "PowerScheme.Tests", Guid.NewGuid().ToString("N"));
        try
        {
            WriteTranslation(directory, "first.xml", "en", "English");
            WriteTranslation(directory, "second.xml", "ru", "Russian");
            WriteTranslation(directory, "third.xml", "de", "Deutsch");
            WriteTranslation(directory, "fourth.xml", "de-at", "Deutsch Austria");
            var expectedDefinition = Language.GetAvailableLanguages(directory).Single(language => language.Code == expected);

            var language = Language.GetLanguage(locale, directory);

            Assert.AreEqual(typeof(Language), language.GetType());
            Assert.AreEqual(expectedDefinition.Strings.Value[nameof(Language.Settings)], language.Settings);
            var code = typeof(Language).GetField("_code", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(language);
            Assert.AreEqual(expected, code);
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }

    [TestMethod]
    public void Discovery_SkipsDuplicateAndInvalidMetadataAndReportsErrors()
    {
        var directory = Path.Combine(Path.GetTempPath(), "PowerScheme.Tests", Guid.NewGuid().ToString("N"));
        var errors = new List<string>();
        try
        {
            WriteTranslation(directory, "a.xml", "de", "Deutsch");
            WriteTranslation(directory, "b.xml", "DE", "Duplicate");
            WriteTranslation(directory, "c.xml", "", "Invalid");
            File.WriteAllText(Path.Combine(directory, "lang", "d.xml"), "<!DOCTYPE resources [<!ENTITY name 'Bad'>]><resources><language code='xx'>&name;</language></resources>");

            var languages = Language.GetAvailableLanguages(directory, errors.Add);

            Assert.AreEqual("Deutsch", languages.Single().Name);
            Assert.AreEqual(3, errors.Count);
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }

    [TestMethod]
    public void LanguageMenu_UsesMetadataAndDiscoversNewFilesOnOpening()
    {
        var original = Language.Code;
        var directory = Path.Combine(Path.GetTempPath(), "PowerScheme.Tests", Guid.NewGuid().ToString("N"));
        try
        {
            Language.SetLanguage("en");
            WriteTranslation(directory, "unrelated.xml", "en", "English from XML");
            var power = new TestPowerService(new TestScheme(Guid.Empty));
            using var menu = new SettingsMenuBuilder(power).CreateLanguageMenu(() => Language.GetAvailableLanguages(directory));
            var english = (ToolStripMenuItem)menu.DropDownItems[0];
            Assert.AreEqual("English from XML", english.Text);
            Assert.AreEqual("en", ((LanguageDefinition)english.Tag!).Code);
            Assert.IsFalse(english.Checked, "The selection must use the transparent image, not the native check renderer.");
            AssertMenuImage(ImageItem.Check, english.Image);

            WriteTranslation(directory, "new.xml", "de", "Deutsch");
            menu.DropDown.Opening += (_, args) => args.Cancel = true;
            menu.ShowDropDown();

            Assert.AreEqual(2, menu.DropDownItems.Count);
            var german = menu.DropDownItems.Cast<ToolStripMenuItem>().Single(item => ((LanguageDefinition)item.Tag!).Code == "de");
            Assert.AreEqual("Deutsch", german.Text);
            Assert.IsFalse(german.Checked);
            AssertMenuImage(ImageItem.Empty, german.Image);
        }
        finally
        {
            Language.SetLanguage(original);
            Directory.Delete(directory, true);
        }
    }

    [TestMethod]
    public void ChangeLanguageCommand_SelectsDynamicTranslationAndSavesCode()
    {
        var original = Language.Code;
        var directory = Path.Combine(Path.GetTempPath(), "PowerScheme.Tests", Guid.NewGuid().ToString("N"));
        try
        {
            WriteTranslation(directory, "custom.xml", "de", "Deutsch");
            var definition = Language.GetAvailableLanguages(directory).Single();
            var power = new TestPowerService(new TestScheme(Guid.Empty));
            string? savedCode = null;
            var icons = 0;
            var command = new ChangeLanguageCommand(power, code => savedCode = code, () => icons++);
            using var item = new ToolStripMenuItem { Tag = definition };

            command.Execute(item, EventArgs.Empty);

            Assert.AreEqual("de", Language.Code);
            Assert.AreEqual("de", savedCode);
            Assert.AreEqual(1, power.RenameCount);
            Assert.AreEqual(1, icons);
            Assert.AreEqual(Language.Current.Settings, MenuLookup.MenuItems[MenuLookup.MenuItm.Settings].Name);
        }
        finally
        {
            Language.SetLanguage(original);
            MenuLookup.RebuildMenuItems();
            Directory.Delete(directory, true);
        }
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void ChangeLanguageCommand_UnreadableTranslationKeepsCurrentLanguage(bool removed)
    {
        var original = Language.Current;
        var directory = Path.Combine(Path.GetTempPath(), "PowerScheme.Tests", Guid.NewGuid().ToString("N"));
        try
        {
            WriteTranslation(directory, "custom.xml", "de", "Deutsch");
            var definition = Language.GetAvailableLanguages(directory).Single();
            var path = Path.Combine(directory, "lang", "custom.xml");
            if (removed)
            {
                File.Delete(path);
            }
            else
            {
                File.WriteAllText(path, "<resources><language code='de'>Deutsch</language><string");
            }
            var power = new TestPowerService(new TestScheme(Guid.Empty));
            var saved = false;
            var updated = false;
            var errors = new List<string>();
            var command = new ChangeLanguageCommand(power, _ => saved = true, () => updated = true, errors.Add);
            using var item = new ToolStripMenuItem { Tag = definition };

            command.Execute(item, EventArgs.Empty);

            Assert.AreSame(original, Language.Current);
            Assert.IsFalse(saved);
            Assert.IsFalse(updated);
            Assert.AreEqual(0, power.RenameCount);
            Assert.AreEqual(1, errors.Count);
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }

    [TestMethod]
    public void SetLanguage_CustomMetadataCode_LoadsFileAndFallsBackForMissingStrings()
    {
        var original = Language.Code;
        var directory = Path.Combine(Path.GetTempPath(), "PowerScheme.Tests", Guid.NewGuid().ToString("N"));
        try
        {
            WriteTranslation(directory, "anything.xml", "de", "Deutsch");
            var path = Path.Combine(directory, "lang", "anything.xml");
            var document = XDocument.Load(path);
            document.Root!.Elements("string").Single(element => (string?)element.Attribute("name") == "Settings").Value = "Einstellungen";
            document.Root.Elements("string").Single(element => (string?)element.Attribute("name") == "Quit").Remove();
            document.Save(path);

            Language.SetLanguage("de", directory);

            Assert.AreEqual("de", Language.Code);
            Assert.AreEqual("Einstellungen", Language.Current.Settings);
            Assert.AreEqual("Quit", Language.Current.Quit);
        }
        finally
        {
            Language.SetLanguage(original);
            Directory.Delete(directory, true);
        }
    }

    [TestMethod]
    public void Discovery_ReadsLanguageTagInsteadOfFilename()
    {
        var directory = Path.Combine(Path.GetTempPath(), "PowerScheme.Tests", Guid.NewGuid().ToString("N"));
        try
        {
            WriteTranslation(directory, "custom-file.xml", "de", "Deutsch");

            var languages = Language.GetAvailableLanguages(directory);

            Assert.AreEqual(1, languages.Count);
            Assert.AreEqual("de", languages[0].Code);
            Assert.AreEqual("Deutsch", languages[0].Name);
            Assert.IsFalse(languages[0].Strings.IsValueCreated);
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }

    [TestMethod]
    public void Discovery_SeesFilesAddedAfterInitialScan()
    {
        var directory = Path.Combine(Path.GetTempPath(), "PowerScheme.Tests", Guid.NewGuid().ToString("N"));
        Assert.AreEqual(0, Language.GetAvailableLanguages(directory).Count);
        try
        {
            WriteTranslation(directory, "new.xml", "de", "Deutsch");

            Assert.AreEqual("de", Language.GetAvailableLanguages(directory).Single().Code);
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }

    private static void AssertMenuImage(ImageItem kind, System.Drawing.Image? actual)
    {
        Assert.IsNotNull(actual);
        using var expected = kind.GetImage();
        Assert.IsInstanceOfType<Bitmap>(actual);
        var bitmap = (Bitmap)actual;
        Assert.AreEqual(expected.Size, bitmap.Size);
        for (var row = 0; row < bitmap.Height; row++)
        {
            for (var column = 0; column < bitmap.Width; column++)
            {
                Assert.AreEqual(expected.GetPixel(column, row).ToArgb(), bitmap.GetPixel(column, row).ToArgb());
            }
        }
    }

    private static void WriteTranslation(string baseDirectory, string filename, string code, string name)
    {
        var directory = Path.Combine(baseDirectory, "lang");
        Directory.CreateDirectory(directory);
        var document = XDocument.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lang", "en.xml"));
        document.Root!.Element("language")!.ReplaceWith(new XElement("language", new XAttribute("code", code), name));
        document.Save(Path.Combine(directory, filename));
    }

    [TestMethod]
    [DataRow("en", "BB05241A3FDC751308C51D5DF73B2661DA5D65220149FF27BBC5C77A8256F5D0")]
    [DataRow("ru", "469ECBAFC9CFC883C3E4FDE0DAB517276685A5A4CEAE624C72098C1E8512EB33")]
    public void Translation_PreservesEveryOriginalString(string locale, string expectedHash)
    {
        var language = Language.GetLanguage(locale);
        var properties = typeof(Language).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .OrderBy(property => property.Name, StringComparer.Ordinal).ToArray();
        Assert.AreEqual(86, properties.Length);
        var canonical = new StringBuilder();
        foreach (var property in properties)
        {
            canonical.Append(property.Name).Append('\0').Append(property.GetValue(language)).Append('\0');
        }

        using var sha = SHA256.Create();
        var hash = BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(canonical.ToString()))).Replace("-", "");
        Assert.AreEqual(expectedHash, hash);
    }

    [TestMethod]
    [DataRow("en")]
    [DataRow("ru")]
    public void ExternalTranslation_IsPlainXmlAndContainsAllKeys(string locale)
    {
        var assembly = typeof(Language).Assembly;
        Assert.AreEqual(0, assembly.GetManifestResourceNames().Length);
        using var resource = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lang", $"{locale}.xml"));
        var document = XDocument.Load(resource);
        var keys = document.Root!.Elements("string").Select(element => (string)element.Attribute("name")!).ToArray();
        var expectedKeys = typeof(Language).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Select(property => property.Name).ToArray();
        CollectionAssert.AreEquivalent(expectedKeys, keys);
    }

    [TestMethod]
    [DataRow("en")]
    [DataRow("ru")]
    public void Loader_ReadsOnlyOnFirstUseAndCachesResult(string locale)
    {
        var selected = LanguageResources.CreateLoader(locale);
        var unused = LanguageResources.CreateLoader("not-selected");
        Assert.IsFalse(selected.IsValueCreated);
        Assert.IsFalse(unused.IsValueCreated);

        var strings = selected.Value;

        Assert.AreEqual(86, strings.Count);
        Assert.IsTrue(selected.IsValueCreated);
        Assert.AreSame(strings, selected.Value);
        Assert.IsFalse(unused.IsValueCreated);
    }

    [TestMethod]
    [DataRow("en")]
    [DataRow("ru")]
    public async Task Loader_ConcurrentFirstAccess_ReturnsOneDictionary(string locale)
    {
        var loader = LanguageResources.CreateLoader(locale);

        var results = await Task.WhenAll(Enumerable.Range(0, 16).Select(_ => Task.Run(() => loader.Value)));

        Assert.IsTrue(results.All(result => ReferenceEquals(results[0], result)));
        Assert.AreEqual(86, results[0].Count);
    }

    [TestMethod]
    public void SetLanguage_SwitchesTranslationsUsingOneResourceModel()
    {
        var originalCode = Language.Code;
        try
        {
            Language.SetLanguage("ru");
            Assert.AreEqual("ru", Language.Code);
            Assert.AreEqual(typeof(Language), Language.Current.GetType());
            var russian = Language.Current.FirstStartDescription;

            Language.SetLanguage("en");
            Assert.AreEqual("en", Language.Code);
            Assert.AreEqual(typeof(Language), Language.Current.GetType());
            Assert.AreEqual("Settings", Language.Current.Settings);
            Assert.AreEqual("Create typical power schemes Stable, Media, Simple?", Language.Current.FirstStartDescription);
            Assert.AreNotEqual(russian, Language.Current.FirstStartDescription);

            Language.SetLanguage("ru");
            Assert.AreEqual(russian, Language.Current.FirstStartDescription);
        }
        finally
        {
            Language.SetLanguage(originalCode);
        }
    }

    [TestMethod]
    public void Loader_UsesApplicationDirectoryNotWorkingDirectory()
    {
        var originalDirectory = Environment.CurrentDirectory;
        var unrelatedDirectory = Path.Combine(Path.GetTempPath(), "PowerScheme.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(unrelatedDirectory);
        try
        {
            Environment.CurrentDirectory = unrelatedDirectory;

            var loader = LanguageResources.CreateLoader("en");

            Assert.AreEqual("Settings", loader.Value[nameof(Language.Settings)]);
            Assert.IsFalse(Directory.Exists(Path.Combine(unrelatedDirectory, "lang")));
        }
        finally
        {
            Environment.CurrentDirectory = originalDirectory;
            Directory.Delete(unrelatedDirectory, true);
        }
    }

    [TestMethod]
    public void Loader_OpensExternalFileOnlyOnDemandAndKeepsCachedStrings()
    {
        var baseDirectory = Path.Combine(Path.GetTempPath(), "PowerScheme.Tests", Guid.NewGuid().ToString("N"));
        var langDirectory = Path.Combine(baseDirectory, "lang");
        Directory.CreateDirectory(langDirectory);
        try
        {
            var loader = LanguageResources.CreateLoader("en", baseDirectory);
            Assert.IsFalse(loader.IsValueCreated);
            var path = Path.Combine(langDirectory, "en.xml");
            File.Copy(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lang", "en.xml"), path);

            var strings = loader.Value;
            File.Delete(path);

            Assert.AreEqual("Settings", strings[nameof(Language.Settings)]);
            Assert.AreSame(strings, loader.Value);
            Assert.AreEqual(86, loader.Value.Count);
        }
        finally
        {
            Directory.Delete(baseDirectory, true);
        }
    }

    [TestMethod]
    public void Loader_MissingResourceFailsOnlyOnAccess()
    {
        var loader = LanguageResources.CreateLoader("missing");
        Assert.IsFalse(loader.IsValueCreated);

        var exception = Assert.ThrowsExactly<FileNotFoundException>(() => _ = loader.Value);

        StringAssert.Contains(exception.Message, "missing.xml");
    }
}