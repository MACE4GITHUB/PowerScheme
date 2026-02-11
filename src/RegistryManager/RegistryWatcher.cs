using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Win32;
using RegistryManager.Common;
using RegistryManager.EventsArgs;
using static RegistryManager.Api.Registry;
using static RegistryManager.NativeMethods;

namespace RegistryManager;

/// <summary>
/// <b>RegistryMonitor</b> allows you to monitor specific registry key.
/// </summary>
/// <remarks>
/// If a monitored registry key changes, an event is fired. You can subscribe to these
/// events by adding a delegate to <see cref="Changed"/>.
/// <para>The Windows API provides a function
/// <a href="http://msdn.microsoft.com/library/en-us/sysinfo/base/regnotifychangekeyvalue.asp">
/// RegNotifyChangeKeyValue</a>, which is not covered by the
/// <see cref="RegistryKey"/> class. <see cref="IRegistryWatcher"/> imports
/// that function and encapsulates it in a convenient manner.
/// </para>
/// </remarks>
/// <example>
/// This sample shows how to monitor <c>HKEY_CURRENT_USER\Environment</c> for changes:
/// <code>
/// public class MonitorSample
/// {
///     static void Main()
///     {
///         RegistryMonitor monitor = new RegistryMonitor(RegistryHive.CurrentUser, "Environment");
///         monitor.Changed += new EventHandler(OnChanged);
///         monitor.Start();
///
///         while(true);
///
///			monitor.Stop();
///     }
///
///     private void OnChanged(object sender, EventArgs e)
///     {
///         Console.WriteLine("registry key has changed");
///     }
/// }
/// </code>
/// </example>
public sealed class RegistryWatcher<T> : IRegistryWatcher, IDisposable
{
    private const int KEY_QUERY_VALUE = 0x0001;
    private const int KEY_NOTIFY = 0x0010;
    private const int STANDARD_RIGHTS_READ = 0x00020000;

    private readonly IntPtr HKEY_CLASSES_ROOT = new(unchecked((int)0x80000000));
    private readonly IntPtr HKEY_CURRENT_USER = new(unchecked((int)0x80000001));
    private readonly IntPtr HKEY_LOCAL_MACHINE = new(unchecked((int)0x80000002));
    private readonly IntPtr HKEY_USERS = new(unchecked((int)0x80000003));
    private readonly IntPtr HKEY_PERFORMANCE_DATA = new(unchecked((int)0x80000004));
    private readonly IntPtr HKEY_CURRENT_CONFIG = new(unchecked((int)0x80000005));

    #region Private member variables

    private IntPtr _registryHive;
    private readonly object _threadLock = new();
    private Thread? _thread;
    private readonly ManualResetEvent _eventTerminate = new(false);
    private readonly RegistryParam _registryParam;
    private readonly RegistryParam _registryParamTemp;
    private RegistryParam _registryParamPrevious;
    private RegistryParam _registryParamCurrent;

    private string[] _subKeysPrevious;

    private RegChangeNotifyFilter _regFilter = RegChangeNotifyFilter.Key | RegChangeNotifyFilter.Attribute |
                                               RegChangeNotifyFilter.Value | RegChangeNotifyFilter.Security;

    #endregion

    #region Event handling

    /// <summary>
    /// Occurs when the specified registry key has changed.
    /// </summary>
    public event EventHandler<RegistryChangedEventArgs>? Changed;

    /// <summary>
    /// Raises the <see cref="Changed"/> event.
    /// </summary>
    /// <remarks>
    /// <p>
    /// <b>OnChanged</b> is called when the specified registry key has changed.
    /// </p>
    /// <note type="inheritinfo">
    /// When overriding <see cref="OnChanged"/> in a derived class, be sure to call
    /// the base class's <see cref="OnChanged"/> method.
    /// </note>
    /// </remarks>
    public void OnChanged()
    {
        if (RegChangeNotifyFilter == RegChangeNotifyFilter.Value)
        {
            _registryParamCurrent.Value = GetSettings<T>(_registryParamTemp);
            if (_registryParamCurrent.Value == _registryParamTemp.Value &&
                _registryParam.Value != _registryParamTemp.Value)
            {
                return;
            }

            if (_registryParamCurrent.Value == _registryParamPrevious.Value)
            {
                return;
            }

            Changed?.Invoke(this, new RegistryChangedEventArgs(RegChangeNotifyFilter, _registryParamPrevious, _registryParamCurrent));

            _registryParamPrevious.Value = _registryParamCurrent.Value;
        }

        if (RegChangeNotifyFilter == RegChangeNotifyFilter.Key)
        {
            var subKeysCurrent = GetSubKeys(_registryParamCurrent).ToArray();

            if (subKeysCurrent.Length == _subKeysPrevious.Length)
            {
                return;
            }

            string[] subKeysExcept;
            bool isChange;

            subKeysExcept =
                subKeysCurrent.Length > _subKeysPrevious.Length ?
                    [.. subKeysCurrent.Except(_subKeysPrevious)] :
                    [.. _subKeysPrevious.Except(subKeysCurrent)];

            isChange = subKeysExcept.Length > 0;
            if (!isChange)
            {
                return;
            }

            // Needs some time to create subKeys / params
            Thread.Sleep(1000);
            Changed?.Invoke(this, new RegistryChangedEventArgs(RegChangeNotifyFilter, _registryParamCurrent, subKeysExcept));
            _subKeysPrevious = subKeysCurrent;
        }
    }

    /// <summary>
    /// Occurs when the access to the registry fails.
    /// </summary>
    public event ErrorEventHandler? Error;

    /// <summary>
    /// Raises the <see cref="Error"/> event.
    /// </summary>
    /// <param name="e">The <see cref="Exception"/> which occured while watching the registry.</param>
    /// <remarks>
    /// <p>
    /// <b>OnError</b> is called when an exception occurs while watching the registry.
    /// </p>
    /// <note type="inheritinfo">
    /// When overriding <see cref="OnError"/> in a derived class, be sure to call
    /// the base class's <see cref="OnError"/> method.
    /// </note>
    /// </remarks>
    public void OnError(Exception e)
    {
        Error?.Invoke(this, new ErrorEventArgs(e));
    }

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="IRegistryWatcher"/> class.
    /// </summary>
    /// <param name="registryParam"></param>
    public RegistryWatcher(RegistryParam registryParam)
    {
        _registryParam = registryParam;

        _registryParamTemp = registryParam;
        _registryParamTemp.Value = null;

        _registryParamPrevious = _registryParamTemp;
        _registryParamPrevious.Value = GetSettings<T>(_registryParamTemp);

        _registryParamCurrent = registryParam;

        _subKeysPrevious = [.. GetSubKeys(registryParam)];

        InitRegistryKey();
    }

    /// <summary>
    /// Disposes this object.
    /// </summary>
    public void Dispose()
    {
        Stop();
    }

    /// <summary>
    /// Gets or sets the <see cref="RegChangeNotifyFilter">RegChangeNotifyFilter</see>.
    /// </summary>
    public RegChangeNotifyFilter RegChangeNotifyFilter
    {
        get => _regFilter;
        set
        {
            lock (_threadLock)
            {
                if (IsMonitoring)
                {
                    throw new InvalidOperationException("Monitoring thread is already running");
                }

                _regFilter = value;
            }
        }
    }

    /// <summary>
    /// <b>true</b> if this <see cref="IRegistryWatcher"/> object is currently monitoring;
    /// otherwise, <b>false</b>.
    /// </summary>
    public bool IsMonitoring => _thread != null;

    /// <summary>
    /// Start monitoring.
    /// </summary>
    public void Start()
    {
        lock (_threadLock)
        {
            if (IsMonitoring)
            {
                return;
            }

            _eventTerminate.Reset();
            _thread = new Thread(MonitorThread)
            {
                IsBackground = true
            };
            _thread.Start();
        }
    }

    /// <summary>
    /// Stops the monitoring thread.
    /// </summary>
    public void Stop()
    {
        lock (_threadLock)
        {
            var thread = _thread;
            if (thread == null)
            {
                return;
            }

            _eventTerminate.Set();
            thread.Join(TimeSpan.FromSeconds(2));

            _thread = null;
        }
    }

    #region Initialization
    private void InitRegistryKey()
    {
        _registryHive = _registryParam.RegistryHive switch
        {
            RegistryHive.ClassesRoot => HKEY_CLASSES_ROOT,
            RegistryHive.CurrentConfig => HKEY_CURRENT_CONFIG,
            RegistryHive.CurrentUser => HKEY_CURRENT_USER,
            RegistryHive.LocalMachine => HKEY_LOCAL_MACHINE,
            RegistryHive.PerformanceData => HKEY_PERFORMANCE_DATA,
            RegistryHive.Users => HKEY_USERS,
            _ => throw new InvalidEnumArgumentException(nameof(_registryParam.RegistryHive),
                (int)_registryParam.RegistryHive, typeof(RegistryHive))
        };
    }
    #endregion

    private void MonitorThread()
    {
        try
        {
            ThreadLoop();
        }
        catch (Exception e)
        {
            OnError(e);
        }
        _thread = null;
    }

    private void ThreadLoop()
    {
        RaiseChanges();

        var result = RegOpenKeyEx(_registryHive, _registryParam.RegistrySubKey, 0, STANDARD_RIGHTS_READ | KEY_QUERY_VALUE | KEY_NOTIFY,
            out var registryKey);

        if (result != 0)
        {
            throw new Win32Exception(result);
        }

        try
        {
            using var eventNotify = new AutoResetEvent(false);
            using var hEvent = eventNotify.SafeWaitHandle;
            WaitHandle[] waitHandles = [eventNotify, _eventTerminate];

            while (true)
            {
                if (_eventTerminate.WaitOne(0))
                {
                    break;
                }

                result = RegNotifyChangeKeyValue(registryKey, false, _regFilter, eventNotify, true);
                if (result != 0)
                {
                    throw new Win32Exception(result);
                }

                var signaled = WaitHandle.WaitAny(waitHandles);
                if (signaled == 0)
                {
                    OnChanged();
                }
                else if (signaled == 1)
                {
                    break;
                }
            }
        }
        finally
        {
            if (registryKey != IntPtr.Zero)
            {
                RegCloseKey(registryKey);
            }
        }
    }

    private void RaiseChanges()
    {
        OnChanged();
    }
}
