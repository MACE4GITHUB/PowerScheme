using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerScheme.Addins.IdleMonitoring;
using PowerScheme.Services;
using PowerScheme.Services.Settings;
using PowerSchemeServiceAPI;
using PowerSchemeServiceAPI.EventsArgs;
using PowerSchemeServiceAPI.Model;
using PowerSchemeServiceAPI.Settings;

namespace PowerScheme.Tests;

[TestClass]
public sealed class IdleMonitorServiceTests
{
    [TestMethod]
    public void Constructor_DoesNotStartMonitoringOrReadSettings()
    {
        using var context = new TestContext();

        Assert.AreEqual(0, context.Monitor.StartCount);
        Assert.AreEqual(0, context.Settings.LoadCount);
    }

    [TestMethod]
    public void Start_ValidOptions_PassesOptionsWithoutWritingDefaults()
    {
        using var context = new TestContext();
        var options = new IdleMonitorOption(30_000, 2_000, 1_000);
        context.Settings.Options = options;

        context.Service.Start();

        Assert.AreSame(options, context.Monitor.Options);
        Assert.AreEqual(1, context.Monitor.StartCount);
        Assert.AreEqual(1, context.Settings.LoadCount);
        Assert.AreEqual(0, context.Settings.SaveCount);
    }

    [TestMethod]
    [DataRow(-1, -1, -1)]
    [DataRow(0, 2_000, 1_000)]
    [DataRow(30_000, 0, 1_000)]
    [DataRow(30_000, 2_000, 6_000)]
    public void Start_InvalidOptions_PersistsAndStartsWithDefaults(int threshold, int activePolling, int idlePolling)
    {
        using var context = new TestContext();
        context.Settings.Options = new IdleMonitorOption(threshold, activePolling, idlePolling);

        context.Service.Start();

        Assert.AreSame(IdleMonitorOption.Default, context.Monitor.Options);
        Assert.AreSame(IdleMonitorOption.Default, context.Settings.Options);
        Assert.AreEqual(1, context.Settings.SaveCount);
    }

    [TestMethod]
    public void Start_AfterStop_ReloadsOptionsAndDoesNotDuplicateSubscriptions()
    {
        using var context = new TestContext();
        context.Service.Start();
        context.Service.Stop();
        var options = new IdleMonitorOption(20_000, 1_000, 1_000);
        context.Settings.Options = options;

        context.Service.Start();
        context.Monitor.RaiseIdle();

        Assert.AreSame(options, context.Monitor.Options);
        Assert.AreEqual(2, context.Settings.LoadCount);
        Assert.AreEqual(2, context.Monitor.StartCount);
        Assert.AreEqual(1, context.Power.Activations.Count);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void OnIdle_SelectsConfiguredSchemeAndOptionallyCopiesBrightnessFirst(bool keepBrightness)
    {
        using var context = new TestContext();
        context.Settings.KeepBrightness = keepBrightness;
        context.Service.Start();

        context.Monitor.RaiseIdle();

        Assert.AreSame(context.Idle, context.Power.ActivePowerScheme);
        Assert.AreEqual(keepBrightness ? 1 : 0, context.Power.BrightnessCopies.Count);
        Assert.AreEqual(keepBrightness ? "brightness" : "activate", context.Power.Operations[0]);
        if (keepBrightness)
        {
            Assert.AreEqual((context.Main.Guid, context.Idle.Guid), context.Power.BrightnessCopies[0]);
        }
    }

    [TestMethod]
    public void OnIdleAndOnActive_RoundTripRestoresMainScheme()
    {
        using var context = new TestContext();
        context.Service.Start();

        context.Monitor.RaiseIdle();
        Assert.AreEqual(context.Main.Guid, context.Settings.MainPowerSchemeId);
        context.Monitor.RaiseActive();

        Assert.AreSame(context.Main, context.Power.ActivePowerScheme);
        Assert.AreEqual(2, context.Power.Activations.Count);
        Assert.AreSame(context.Idle, context.Power.Activations[0]);
        Assert.AreSame(context.Main, context.Power.Activations[1]);
        Assert.AreEqual(0, context.Settings.MainWriteCount);
    }

    [TestMethod]
    public void OnActive_RestoresSavedMainScheme()
    {
        using var context = new TestContext();
        context.Settings.MainSchemeId = context.Other.Guid;
        context.Service.Start();

        context.Monitor.RaiseActive();

        Assert.AreSame(context.Other, context.Power.ActivePowerScheme);
        Assert.AreEqual(0, context.Settings.MainWriteCount);
    }

    [TestMethod]
    public void OnActive_MissingMainScheme_UsesFirstNativeScheme()
    {
        using var context = new TestContext();
        context.Settings.MainSchemeId = Guid.NewGuid();
        context.Power.NativeSchemes = [context.Other, context.Main];
        context.Service.Start();

        context.Monitor.RaiseActive();

        Assert.AreSame(context.Other, context.Power.ActivePowerScheme);
        Assert.AreEqual(context.Other.Guid, context.Settings.MainPowerSchemeId);
        Assert.AreEqual(1, context.Settings.MainWriteCount);
    }

    [TestMethod]
    public void OnIdle_MissingIdleScheme_UsesLastEligibleNativeScheme()
    {
        using var context = new TestContext();
        context.Settings.IdlePowerSchemeId = Guid.NewGuid();
        var extreme = new TestScheme(Guid.NewGuid(), true, true);
        var custom = new TestScheme(Guid.NewGuid(), false);
        context.Power.NativeSchemes = [context.Main, context.Other, extreme, custom];
        context.Service.Start();

        context.Monitor.RaiseIdle();

        Assert.AreSame(context.Other, context.Power.ActivePowerScheme);
    }

    [TestMethod]
    public void DisabledMonitoring_IgnoresIdleActiveAndSchemeChangeEvents()
    {
        using var context = new TestContext();
        context.Settings.IdlePowerSchemeId = Guid.Empty;
        context.Settings.KeepBrightness = true;
        context.Service.Start();

        context.Monitor.RaiseIdle();
        context.Monitor.RaiseActive();
        context.Power.RaiseSchemeChanged(context.Main, context.Other);

        Assert.AreEqual(0, context.Power.Activations.Count);
        Assert.AreEqual(0, context.Power.BrightnessCopies.Count);
        Assert.AreEqual(0, context.Settings.MainWriteCount);
    }

    [TestMethod]
    public void OnIdle_ReadsChangedSchemeAndBrightnessSettingsWithoutRestart()
    {
        using var context = new TestContext();
        context.Service.Start();
        context.Monitor.RaiseIdle();
        context.Monitor.RaiseActive();
        context.Settings.IdlePowerSchemeId = context.Other.Guid;
        context.Settings.KeepBrightness = true;

        context.Monitor.RaiseIdle();

        Assert.AreSame(context.Other, context.Power.ActivePowerScheme);
        Assert.AreEqual((context.Main.Guid, context.Other.Guid), context.Power.BrightnessCopies[0]);
        Assert.AreEqual(1, context.Monitor.StartCount);
    }

    [TestMethod]
    public void OnActive_ReadsChangedMainSettingWithoutRestart()
    {
        using var context = new TestContext();
        context.Service.Start();
        context.Monitor.RaiseIdle();
        context.Settings.MainSchemeId = context.Other.Guid;

        context.Monitor.RaiseActive();

        Assert.AreSame(context.Other, context.Power.ActivePowerScheme);
        Assert.AreEqual(1, context.Monitor.StartCount);
    }

    [TestMethod]
    public void SchemeChanged_EnteringIdle_RemembersPreviousScheme()
    {
        using var context = new TestContext();
        context.Settings.MainSchemeId = context.Other.Guid;
        context.Service.Start();

        context.Power.SetActivePowerScheme(context.Idle);

        Assert.AreEqual(context.Main.Guid, context.Settings.MainPowerSchemeId);
        Assert.AreEqual(1, context.Settings.MainWriteCount);
    }

    [TestMethod]
    public void SchemeChanged_SelectingIdleAgain_RecordsIdleAsMainScheme()
    {
        using var context = new TestContext();
        context.Service.Start();
        context.Power.SetActivePowerScheme(context.Idle);

        context.Power.SetActivePowerScheme(context.Idle);

        Assert.AreEqual(context.Idle.Guid, context.Settings.MainPowerSchemeId);
        Assert.AreEqual(1, context.Settings.MainWriteCount);
    }

    [TestMethod]
    public void SchemeChanged_SelectingNonIdleScheme_UpdatesMainOnlyOnce()
    {
        using var context = new TestContext();
        context.Service.Start();

        context.Power.SetActivePowerScheme(context.Other);
        context.Power.SetActivePowerScheme(context.Other);

        Assert.AreEqual(context.Other.Guid, context.Settings.MainPowerSchemeId);
        Assert.AreEqual(1, context.Settings.MainWriteCount);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void StopOrDispose_UnsubscribesAllEvents(bool dispose)
    {
        using var context = new TestContext();
        context.Service.Start();
        if (dispose)
        {
            context.Service.Dispose();
        }
        else
        {
            context.Service.Stop();
        }

        context.Monitor.RaiseIdle();
        context.Monitor.RaiseActive();
        context.Power.RaiseSchemeChanged(context.Main, context.Other);

        Assert.AreEqual(1, context.Monitor.StopCount);
        Assert.AreEqual(0, context.Power.Activations.Count);
        Assert.AreEqual(0, context.Settings.MainWriteCount);
    }

    private sealed class TestContext : IDisposable
    {
        public TestScheme Main { get; } = new(Guid.NewGuid());
        public TestScheme Idle { get; } = new(Guid.NewGuid());
        public TestScheme Other { get; } = new(Guid.NewGuid());
        public TestIdleMonitor Monitor { get; } = new();
        public TestSettingsStore Settings { get; } = new();
        public TestPowerService Power { get; }
        public IdleMonitorService Service { get; }

        public TestContext()
        {
            Settings.IdlePowerSchemeId = Idle.Guid;
            Settings.MainSchemeId = Main.Guid;
            Power = new TestPowerService(Main)
            {
                Schemes = [Main, Idle, Other],
                NativeSchemes = [Main, Idle]
            };
            Service = new IdleMonitorService(Monitor, Power, Settings);
        }

        public void Dispose() => Service.Dispose();
    }

    private sealed class TestSettingsStore : IIdleSettingsStore
    {
        public IdleMonitorOption Options { get; set; } = IdleMonitorOption.Default;
        public int LoadCount { get; private set; }
        public int SaveCount { get; private set; }
        public Guid IdlePowerSchemeId { get; set; }
        public Guid MainSchemeId { get; set; }
        public bool KeepBrightness { get; set; }
        public int MainWriteCount { get; private set; }

        public Guid MainPowerSchemeId
        {
            get => MainSchemeId;
            set
            {
                MainSchemeId = value;
                MainWriteCount++;
            }
        }

        public IdleMonitorOption LoadMonitorOptions()
        {
            LoadCount++;
            return Options;
        }

        public void SaveMonitorOptions(IdleMonitorOption options)
        {
            SaveCount++;
            Options = options;
        }
    }

    private sealed class TestIdleMonitor : IIdleMonitor
    {
        public event EventHandler? OnIdle;
        public event EventHandler? OnActive;
        public IdleMonitorOption? Options { get; private set; }
        public int StartCount { get; private set; }
        public int StopCount { get; private set; }

        public Task StartAsync(IdleMonitorOption idleMonitorOption)
        {
            Options = idleMonitorOption;
            StartCount++;
            return Task.CompletedTask;
        }

        public void Stop() => StopCount++;
        public Task WaitForStopAsync() => Task.CompletedTask;
        public void RaiseIdle() => OnIdle?.Invoke(this, EventArgs.Empty);
        public void RaiseActive() => OnActive?.Invoke(this, EventArgs.Empty);
    }

    private sealed class TestScheme(Guid guid, bool isNative = true, bool isMaxPerformance = false) : IPowerScheme
    {
        public Guid Guid { get; } = guid;
        public bool IsNative { get; } = isNative;
        public bool IsMaxPerformance { get; } = isMaxPerformance;
        public bool IsVisible => true;
        public bool IsActive => throw new NotSupportedException();
        public string Name => throw new NotSupportedException();
        public string? Description => throw new NotSupportedException();
        public ImageItem Picture => throw new NotSupportedException();
    }

    private sealed class TestPowerService(IPowerScheme initialScheme) : IPowerSchemeService
    {
        public event EventHandler<PowerSchemeEventArgs> ActivePowerSchemeChanged = delegate { };
        public IPowerScheme ActivePowerScheme { get; private set; } = initialScheme;
        public IEnumerable<IPowerScheme> Schemes { get; set; } = [];
        public IEnumerable<IPowerScheme> NativeSchemes { get; set; } = [];
        public IEnumerable<IPowerScheme> PowerSchemes => Schemes;
        public IEnumerable<IPowerScheme> PowerProfPowerSchemes => NativeSchemes;
        public List<IPowerScheme> Activations { get; } = [];
        public List<(Guid Source, Guid Destination)> BrightnessCopies { get; } = [];
        public List<string> Operations { get; } = [];

        public void SetActivePowerScheme(IPowerScheme newPowerScheme, bool isForce = false)
        {
            var previous = ActivePowerScheme;
            ActivePowerScheme = newPowerScheme;
            Activations.Add(newPowerScheme);
            Operations.Add("activate");
            if (!isForce)
            {
                RaiseSchemeChanged(previous, newPowerScheme);
            }
        }

        public void RaiseSchemeChanged(IPowerScheme previous, IPowerScheme active) =>
            ActivePowerSchemeChanged.Invoke(this, new PowerSchemeEventArgs(previous, active));

        public void CopyBrightness(Guid sourcePowerScheme, Guid destinationPowerScheme)
        {
            BrightnessCopies.Add((sourcePowerScheme, destinationPowerScheme));
            Operations.Add("brightness");
        }

        public IEnumerable<IPowerScheme> TypicalPowerSchemesWithDeleted => throw new NotSupportedException();
        public IEnumerable<IPowerScheme> TypicalPowerSchemesWithoutDeleted => throw new NotSupportedException();
        public IEnumerable<IPowerScheme> UserPowerSchemes => throw new NotSupportedException();
        public IEnumerable<IPowerScheme> CustomPowerSchemes => throw new NotSupportedException();
        public Watchers Watchers => throw new NotSupportedException();
        public bool IsNeedAdminAccessForChangePowerScheme => throw new NotSupportedException();
        public bool ExistsMobilePlatformRole => throw new NotSupportedException();
        public bool ExistsHibernate => throw new NotSupportedException();
        public bool ExistsSleep => throw new NotSupportedException();
        public bool ExistsAllTypicalScheme => throw new NotSupportedException();
        public void RestoreDefaultPowerSchemes() => throw new NotSupportedException();
        public void DeleteAllTypicalScheme() => throw new NotSupportedException();
        public void CreateTypicalSchemes() => throw new NotSupportedException();
        public void RenameTypicalSchemes() => throw new NotSupportedException();
        public void SetLid(int value) => throw new NotSupportedException();
        public string TextActionToggle(StatePowerScheme statePowerScheme) => throw new NotSupportedException();
        public StatePowerScheme? StatePowerSchemeToggle(StatePowerScheme statePowerScheme) => throw new NotSupportedException();
        public void ActionPowerScheme(StatePowerScheme statePowerScheme) => throw new NotSupportedException();
        public void DeletePowerScheme(StatePowerScheme statePowerScheme) => throw new NotSupportedException();
        public void SetAllPowerSchemesIdleDisplay(int value) => throw new NotSupportedException();
        public void SetIdleDisplay(Guid guid, int value) => throw new NotSupportedException();
        public PowerSchemeDcAcValues GetIdleDisplay(Guid guid) => throw new NotSupportedException();
        public void SetAllPowerSchemesIdleLockDisplay(int value) => throw new NotSupportedException();
        public void SetIdleLockDisplay(Guid guid, int value) => throw new NotSupportedException();
        public PowerSchemeDcAcValues GetIdleLockDisplay(Guid guid) => throw new NotSupportedException();
        public void SetAllPowerSchemesIdleSleep(int value) => throw new NotSupportedException();
        public void SetIdleSleep(Guid guid, int value) => throw new NotSupportedException();
        public PowerSchemeDcAcValues GetIdleSleep(Guid guid) => throw new NotSupportedException();
        public void SetAllPowerSchemesIdleHibernate(int value) => throw new NotSupportedException();
        public void SetIdleHibernate(Guid guid, int value) => throw new NotSupportedException();
        public PowerSchemeDcAcValues GetIdleHibernate(Guid guid) => throw new NotSupportedException();
    }
}
