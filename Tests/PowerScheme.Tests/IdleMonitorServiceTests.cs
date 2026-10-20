using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerScheme.Addins.IdleMonitoring;
using PowerScheme.Services;
using PowerScheme.Services.Settings;
using PowerScheme.Tests.TestDoubles;

namespace PowerScheme.Tests;

[TestClass]
public sealed class IdleMonitorServiceTests
{
    [TestMethod]
    public async Task RestartAsync_WaitsForActualStopAndReloadsLatestOptions()
    {
        using var context = new TestContext();
        context.Service.Start();
        var stopped = new TaskCompletionSource<bool>();
        context.Monitor.StopTask = stopped.Task;

        var restart = context.Service.RestartAsync();

        Assert.IsFalse(restart.IsCompleted);
        Assert.AreEqual(1, context.Monitor.StartCount);
        context.Monitor.RaiseIdle();
        Assert.AreEqual(0, context.Power.Activations.Count);
        var options = new IdleMonitorOption(20_000, 1_000, 1_000);
        context.Settings.Options = options;
        stopped.SetResult(true);
        await restart;

        Assert.AreSame(options, context.Monitor.Options);
        Assert.AreEqual(2, context.Monitor.StartCount);
        context.Monitor.RaiseIdle();
        Assert.AreEqual(1, context.Power.Activations.Count);
    }

    [TestMethod]
    public async Task RestartAsync_RepeatedRequestsShareOneRestart()
    {
        using var context = new TestContext();
        context.Service.Start();
        var stopped = new TaskCompletionSource<bool>();
        context.Monitor.StopTask = stopped.Task;

        var first = context.Service.RestartAsync();
        var second = context.Service.RestartAsync();

        Assert.AreSame(first, second);
        Assert.AreEqual(1, context.Monitor.StopCount);
        stopped.SetResult(true);
        await Task.WhenAll(first, second);
        Assert.AreEqual(2, context.Monitor.StartCount);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task StopOrDispose_DuringRestart_PreventsDeferredStart(bool dispose)
    {
        using var context = new TestContext();
        context.Service.Start();
        var stopped = new TaskCompletionSource<bool>();
        context.Monitor.StopTask = stopped.Task;
        var restart = context.Service.RestartAsync();

        if (dispose)
        {
            context.Service.Dispose();
        }
        else
        {
            context.Service.Stop();
        }

        stopped.SetResult(true);
        await restart;
        context.Monitor.RaiseIdle();
        Assert.AreEqual(1, context.Monitor.StartCount);
        Assert.AreEqual(0, context.Power.Activations.Count);
    }

    [TestMethod]
    public async Task RestartAsync_AfterCanceledRestart_OnlyNewestRequestStarts()
    {
        using var context = new TestContext();
        context.Service.Start();
        var stopped = new TaskCompletionSource<bool>();
        context.Monitor.StopTask = stopped.Task;
        var first = context.Service.RestartAsync();
        context.Service.Stop();

        var second = context.Service.RestartAsync();
        stopped.SetResult(true);
        await Task.WhenAll(first, second);

        Assert.AreEqual(2, context.Monitor.StartCount);
    }

    [TestMethod]
    public async Task RestartAsync_StartFailure_UnsubscribesAndAllowsRetry()
    {
        using var context = new TestContext();
        context.Service.Start();
        var expected = new InvalidOperationException("Start failed.");
        context.Monitor.StartTask = Task.FromException(expected);

        var actual = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => context.Service.RestartAsync());

        Assert.AreSame(expected, actual);
        context.Monitor.RaiseIdle();
        Assert.AreEqual(0, context.Power.Activations.Count);
        context.Monitor.StartTask = Task.CompletedTask;
        await context.Service.RestartAsync();
        context.Monitor.RaiseIdle();
        Assert.AreEqual(1, context.Power.Activations.Count);
    }

    [TestMethod]
    public async Task Start_DuringPendingRestart_DoesNotStartAnotherCycle()
    {
        using var context = new TestContext();
        context.Service.Start();
        var stopped = new TaskCompletionSource<bool>();
        context.Monitor.StopTask = stopped.Task;
        var restart = context.Service.RestartAsync();

        Assert.ThrowsExactly<InvalidOperationException>(() => context.Service.Start());

        stopped.SetResult(true);
        await restart;
        Assert.AreEqual(2, context.Monitor.StartCount);
    }

    [TestMethod]
    public void Start_RepeatedCall_DoesNotDuplicateMonitoringOrSubscriptions()
    {
        using var context = new TestContext();
        context.Service.Start();
        context.Service.Start();
        context.Monitor.RaiseIdle();

        Assert.AreEqual(1, context.Monitor.StartCount);
        Assert.AreEqual(1, context.Power.Activations.Count);
    }

    [TestMethod]
    public void Dispose_RejectsFurtherStartsAndRestarts()
    {
        using var context = new TestContext();
        context.Service.Dispose();

        Assert.ThrowsExactly<ObjectDisposedException>(() => context.Service.Start());
        Assert.ThrowsExactly<ObjectDisposedException>(() => context.Service.RestartAsync());
    }

    [TestMethod]
    public async Task RestartAsync_WaitsForPendingInitialStartAndThenForStop()
    {
        using var context = new TestContext();
        var started = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var stopped = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        context.Monitor.StartTask = started.Task;
        context.Service.Start();
        context.Monitor.StopTask = stopped.Task;

        var restart = context.Service.RestartAsync();

        Assert.IsFalse(restart.IsCompleted);
        Assert.AreEqual(1, context.Monitor.StartCount);
        context.Monitor.StartTask = Task.CompletedTask;
        started.SetResult(true);
        Assert.IsFalse(restart.IsCompleted);
        stopped.SetResult(true);
        await restart;
        Assert.AreEqual(2, context.Monitor.StartCount);
    }

    [TestMethod]
    public async Task RestartAsync_AsyncStartFailure_IsAwaitedAndAllowsRetry()
    {
        using var context = new TestContext();
        context.Service.Start();
        var started = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var expected = new InvalidOperationException("Asynchronous start failed.");
        context.Monitor.StartTask = started.Task;

        var restart = context.Service.RestartAsync();

        Assert.IsFalse(restart.IsCompleted);
        Assert.AreEqual(2, context.Monitor.StartCount);
        started.SetException(expected);
        var actual = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => restart);
        Assert.AreSame(expected, actual);
        context.Monitor.RaiseIdle();
        Assert.AreEqual(0, context.Power.Activations.Count);
        context.Monitor.StartTask = Task.CompletedTask;
        await context.Service.RestartAsync();
        context.Monitor.RaiseIdle();
        Assert.AreEqual(1, context.Power.Activations.Count);
    }

    [TestMethod]
    public async Task RestartAsync_StopWaitFailure_DoesNotStartAndAllowsRetry()
    {
        using var context = new TestContext();
        context.Service.Start();
        var expected = new InvalidOperationException("Stop wait failed.");
        context.Monitor.StopTask = Task.FromException(expected);

        var actual = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => context.Service.RestartAsync());

        Assert.AreSame(expected, actual);
        Assert.AreEqual(1, context.Monitor.StartCount);
        context.Monitor.StopTask = Task.CompletedTask;
        await context.Service.RestartAsync();
        Assert.AreEqual(2, context.Monitor.StartCount);
    }

    [TestMethod]
    public async Task Stop_DuringNewStartup_DoesNotResubscribeWhenStartupCompletes()
    {
        using var context = new TestContext();
        context.Service.Start();
        var started = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        context.Monitor.StartTask = started.Task;
        var restart = context.Service.RestartAsync();

        context.Service.Stop();
        started.SetResult(true);
        await restart;
        context.Monitor.RaiseIdle();

        Assert.AreEqual(2, context.Monitor.StartCount);
        Assert.AreEqual(0, context.Power.Activations.Count);
    }

    [TestMethod]
    public async Task RestartAsync_RealMonitor_StopsEachCycleBeforeStartingNext()
    {
        using var context = new TestContext();
        var detector = new TestIdleDetector();
        var monitor = new IdleMonitor(detector);
        using var service = new IdleMonitorService(monitor, context.Power, context.Settings);
        try
        {
            service.Start();
            await service.RestartAsync();
            await service.RestartAsync();
        }
        finally
        {
            service.Stop();
            await monitor.WaitForStopAsync();
        }

        Assert.AreEqual(3, detector.SampleCount);
        Assert.AreEqual(0, context.Power.Activations.Count);
    }

    [TestMethod]
    public async Task Start_AfterStopWithPendingStartup_DoesNotStartAnotherCycle()
    {
        using var context = new TestContext();
        var started = new TaskCompletionSource<bool>();
        context.Monitor.StartTask = started.Task;
        context.Service.Start();
        context.Service.Stop();

        Assert.ThrowsExactly<InvalidOperationException>(() => context.Service.Start());
        Assert.AreEqual(1, context.Monitor.StartCount);

        started.SetResult(true);
        context.Monitor.StartTask = Task.CompletedTask;
        await context.Service.RestartAsync();
        Assert.AreEqual(2, context.Monitor.StartCount);
    }

    [TestMethod]
    public async Task RestartAsync_DuringNewStartup_RestartsAgainWithLatestOptions()
    {
        using var context = new TestContext();
        context.Service.Start();
        var started = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        context.Monitor.StartTask = started.Task;
        var first = context.Service.RestartAsync();
        var latest = new IdleMonitorOption(25_000, 1_000, 1_000);
        context.Settings.Options = latest;

        var second = context.Service.RestartAsync();

        Assert.AreNotSame(first, second);
        Assert.AreEqual(2, context.Monitor.StartCount);
        context.Monitor.StartTask = Task.CompletedTask;
        started.SetResult(true);
        await Task.WhenAll(first, second);
        Assert.AreEqual(3, context.Monitor.StartCount);
        Assert.AreSame(latest, context.Monitor.Options);
        context.Monitor.RaiseIdle();
        Assert.AreEqual(1, context.Power.Activations.Count);
    }

    [TestMethod]
    public async Task RestartAsync_ConcurrentRequests_WaitOnOneStoppedCycle()
    {
        using var context = new TestContext();
        context.Service.Start();
        var stopped = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        context.Monitor.StopTask = stopped.Task;
        Task? first = null;
        Task? second = null;

        await Task.WhenAll(
            Task.Run(() => { first = context.Service.RestartAsync(); }),
            Task.Run(() => { second = context.Service.RestartAsync(); }));

        Assert.AreSame(first, second);
        Assert.AreEqual(1, context.Monitor.StopCount);
        stopped.SetResult(true);
        await first!;
        Assert.AreEqual(2, context.Monitor.StartCount);
    }

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

    private sealed class TestIdleDetector : IIdleDetector
    {
        private int _sampleCount;
        public int SampleCount => Volatile.Read(ref _sampleCount);

        public TimeSpan GetIdleTime()
        {
            Interlocked.Increment(ref _sampleCount);
            return TimeSpan.Zero;
        }
    }

    private sealed class TestIdleMonitor : IIdleMonitor
    {
        public event EventHandler? OnIdle;
        public event EventHandler? OnActive;
        public IdleMonitorOption? Options { get; private set; }
        public int StartCount { get; private set; }
        public int StopCount { get; private set; }
        public Task StopTask { get; set; } = Task.CompletedTask;
        public Task StartTask { get; set; } = Task.CompletedTask;

        public Task StartAsync(IdleMonitorOption idleMonitorOption)
        {
            Options = idleMonitorOption;
            StartCount++;
            return StartTask;
        }

        public void Stop() => StopCount++;
        public Task WaitForStopAsync() => StopTask;
        public void RaiseIdle() => OnIdle?.Invoke(this, EventArgs.Empty);
        public void RaiseActive() => OnActive?.Invoke(this, EventArgs.Empty);
    }

}
