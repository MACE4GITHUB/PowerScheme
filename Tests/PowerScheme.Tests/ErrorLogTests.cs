using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Languages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerScheme.Utility;

namespace PowerScheme.Tests;

[TestClass]
[DoNotParallelize]
public sealed class ErrorLogTests
{
    private string _directory = null!;
    private string LogPath => Path.Combine(_directory, "errors.log");

    [TestInitialize]
    public void Initialize() => _directory = Path.Combine(Path.GetTempPath(), "PowerScheme.Tests", Guid.NewGuid().ToString("N"));

    [TestCleanup]
    public void Cleanup()
    {
        if (Directory.Exists(_directory))
        {
            Directory.Delete(_directory, true);
        }
        else if (File.Exists(_directory))
        {
            File.Delete(_directory);
        }
    }

    [TestMethod]
    public void DirectoryPath_UsesLocalApplicationData() => Assert.AreEqual(
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PowerScheme"),
        ErrorLog.DirectoryPath);

    [TestMethod]
    public void Startup_SuccessDoesNotReportAnError()
    {
        var executed = false;
        var reported = false;

        var success = Program.TryRunStartup(() => executed = true, _ => reported = true);

        Assert.IsTrue(success);
        Assert.IsTrue(executed);
        Assert.IsFalse(reported);
    }

    [TestMethod]
    public void Startup_MissingDependencyCreatesLogWithFullException()
    {
        var success = Program.TryRunStartup(
            () => throw new FileNotFoundException("Required dependency is missing.", "MissingDependency.dll"),
            message => ErrorLog.Write(message, _directory));

        Assert.IsFalse(success);
        var contents = File.ReadAllText(LogPath);
        StringAssert.Contains(contents, "[Startup]");
        StringAssert.Contains(contents, "System.IO.FileNotFoundException");
        StringAssert.Contains(contents, "MissingDependency.dll");
        StringAssert.Contains(contents, "Required dependency is missing.");
    }

    [TestMethod]
    public void Startup_MissingTranslationsLogsActualTypeInitializationFailure()
    {
        Directory.CreateDirectory(_directory);
        var domain = AppDomain.CreateDomain("Missing translations", null, new AppDomainSetup
        {
            ApplicationBase = _directory
        });
        try
        {
            var probe = (LanguageStartupProbe)domain.CreateInstanceFromAndUnwrap(
                typeof(LanguageStartupProbe).Assembly.Location, typeof(LanguageStartupProbe).FullName);

            var success = Program.TryRunStartup(() => probe.LoadLanguage(typeof(Language).Assembly.Location),
                message => ErrorLog.Write(message, _directory));

            Assert.IsFalse(success);
            var contents = File.ReadAllText(LogPath);
            StringAssert.Contains(contents, "[Startup]");
            StringAssert.Contains(contents, "System.TypeInitializationException");
            StringAssert.Contains(contents, "Languages.Language");
            StringAssert.Contains(contents, "No valid translations found in the lang directory.");
        }
        finally
        {
            AppDomain.Unload(domain);
        }
    }

    public sealed class LanguageStartupProbe : MarshalByRefObject
    {
        public void LoadLanguage(string assemblyPath) =>
            Assembly.LoadFrom(assemblyPath).GetType("Languages.Language", true)!
                .GetProperty("Current", BindingFlags.Public | BindingFlags.Static)!.GetValue(null);
    }

    [TestMethod]
    public void Write_CreatesDirectoryAndAppendsMessages()
    {
        ErrorLog.Write("first message", _directory);
        ErrorLog.Write("second message", _directory);

        var contents = File.ReadAllText(LogPath);
        StringAssert.Contains(contents, "first message");
        StringAssert.Contains(contents, "second message");
        Assert.AreEqual(2, File.ReadAllLines(LogPath).Length);
        CollectionAssert.AreEqual(new[] { "errors.log" }, Directory.GetFiles(_directory).Select(Path.GetFileName).ToArray());
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(100)]
    public void Write_FullOrOversizedExistingFile_ReplacesOldContents(int extraBytes)
    {
        Directory.CreateDirectory(_directory);
        using (var stream = File.Create(LogPath))
        {
            stream.SetLength(ErrorLog.MaxFileBytes + extraBytes);
        }

        ErrorLog.Write("latest message", _directory);

        StringAssert.Contains(File.ReadAllText(LogPath), "latest message");
        Assert.IsLessThan(1024L, new FileInfo(LogPath).Length);
    }

    [TestMethod]
    [DataRow('x')]
    [DataRow('\u0416')]
    public void Write_OversizedMessage_RespectsByteLimitAndValidUtf8(char character)
    {
        ErrorLog.Write(new string(character, ErrorLog.MaxFileBytes + 100), _directory);

        Assert.IsLessThanOrEqualTo((long)ErrorLog.MaxFileBytes, new FileInfo(LogPath).Length);
        var contents = new UTF8Encoding(false, true).GetString(File.ReadAllBytes(LogPath));
        Assert.IsTrue(contents.EndsWith(Environment.NewLine, StringComparison.Ordinal));
        StringAssert.Contains(contents, new string(character, 100));
    }

    [TestMethod]
    public void Write_AfterOversizedMessage_KeepsNewestEntry()
    {
        ErrorLog.Write(new string('x', ErrorLog.MaxFileBytes), _directory);

        ErrorLog.Write("newest entry", _directory);

        var contents = File.ReadAllText(LogPath);
        StringAssert.Contains(contents, "newest entry");
        Assert.IsFalse(contents.Contains(new string('x', 100)));
    }

    [TestMethod]
    public async Task Write_ConcurrentMessages_DoNotExceedLimitOrCorruptLines()
    {
        await Task.WhenAll(Enumerable.Range(0, 32).Select(index => Task.Run(() =>
            ErrorLog.Write($"entry-{index}:" + new string('x', 60_000), _directory))));

        Assert.IsLessThanOrEqualTo((long)ErrorLog.MaxFileBytes, new FileInfo(LogPath).Length);
        var lines = File.ReadAllLines(LogPath);
        Assert.IsTrue(lines.Length > 0);
        Assert.IsTrue(lines.All(line => line.Contains("entry-") && line.EndsWith(new string('x', 60_000), StringComparison.Ordinal)));
    }

    [TestMethod]
    public void Write_UnavailableDirectory_DoesNotThrow()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_directory)!);
        File.WriteAllText(_directory, "Not a directory.");

        ErrorLog.Write("error", _directory);

        Assert.AreEqual("Not a directory.", File.ReadAllText(_directory));
    }
}