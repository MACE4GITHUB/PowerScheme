using RegistryManager.Model;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace RegistryManager;

[ComVisible(false), System.Security.SuppressUnmanagedCodeSecurity()]
internal static class NativeMethods
{
    private const string ADVAPI32 = "advapi32.dll";
    private const string KERNEL32 = "kernel32.dll";

    [DllImport(ADVAPI32, SetLastError = true)]
    public static extern int RegOpenKeyEx(IntPtr hKey, string subKey, uint options, int samDesired,
        out IntPtr phkResult);

    [DllImport(ADVAPI32, SetLastError = true)]
    public static extern int RegNotifyChangeKeyValue(IntPtr hKey, bool bWatchSubtree,
        RegChangeNotifyFilter dwNotifyFilter, SafeEventHandle hEvent,
        bool fAsynchronous);

    [DllImport(ADVAPI32, SetLastError = true)]
    public static extern int RegCloseKey(IntPtr hKey);

    [DllImport(KERNEL32, SetLastError = true, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ResetEvent([In] SafeEventHandle hEvent);

    [DllImport(KERNEL32, SetLastError = true, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetEvent([In] SafeEventHandle hEvent);

    [DllImport(KERNEL32, SetLastError = true, ExactSpelling = true)]
    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CloseHandle(IntPtr hObject);

    /// <summary>Provides a <see cref="SafeHandle"/> to an event that is automatically disposed using CloseHandle.</summary>
    public class SafeEventHandle : SafeSyncHandle
    {
        /// <summary>Initializes a new instance of the <see cref="SafeEventHandle"/> class and assigns an existing handle.</summary>
        /// <param name="preexistingHandle">An <see cref="IntPtr"/> object that represents the pre-existing handle to use.</param>
        /// <param name="ownsHandle">
        /// <see langword="true"/> to reliably release the handle during the finalization phase; otherwise, <see langword="false"/> (not recommended).
        /// </param>
        public SafeEventHandle(IntPtr preexistingHandle, bool ownsHandle = true) : base(preexistingHandle, ownsHandle) { }

        private SafeEventHandle() : base() { }

        /// <summary>Sets this event object to the nonsignaled state.</summary>
        /// <returns>
        /// <para>If the function succeeds, the return value is nonzero.</para>
        /// <para>If the function fails, the return value is zero. To get extended error information, call <c>GetLastError</c>.</para>
        /// </returns>
        public bool Reset() => ResetEvent(this);

        /// <summary>Sets this event object to the signaled state.</summary>
        /// <returns>
        /// <para>If the function succeeds, the return value is nonzero.</para>
        /// <para>If the function fails, the return value is zero. To get extended error information, call <c>GetLastError</c>.</para>
        /// </returns>
        public bool Set() => SetEvent(this);

#pragma warning disable CS0618 // Type or member is obsolete
        /// <summary>Performs an implicit conversion from <see cref="EventWaitHandle"/> to <see cref="SafeWaitHandle"/>.</summary>
        /// <param name="h">The SafeSyncHandle instance.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator SafeEventHandle(EventWaitHandle h) => new(h.Handle, false);
#pragma warning restore CS0618 // Type or member is obsolete

        /// <summary>Gets an invalid event handle.</summary>
        public static SafeEventHandle InvalidHandle => new(new IntPtr(-1), false);

        /// <summary>Gets a null event handle.</summary>
        public static SafeEventHandle Null => new(IntPtr.Zero, false);

        /// <summary>Performs an explicit conversion from <see cref="SafeEventHandle"/> to <see cref="IntPtr"/>.</summary>
        /// <param name="h">The event handle.</param>
        /// <returns>The resulting <see cref="IntPtr"/> instance from the conversion.</returns>
        public static explicit operator IntPtr(SafeEventHandle h) => h.handle;
    }

    /// <summary>Provides a <see cref="SafeHandle"/> to a synchronization object that is automatically disposed using CloseHandle.</summary>
    /// <remarks></remarks>
    public abstract class SafeSyncHandle : SafeKernelHandle, ISyncHandle
    {
        /// <summary>Initializes a new instance of the <see cref="SafeSyncHandle"/> class.</summary>
        protected SafeSyncHandle() : base() { }

        /// <summary>Initializes a new instance of the <see cref="SafeSyncHandle"/> class and assigns an existing handle.</summary>
        /// <param name="preexistingHandle">An <see cref="IntPtr"/> object that represents the pre-existing handle to use.</param>
        /// <param name="ownsHandle">
        /// <see langword="true"/> to reliably release the handle during the finalization phase; otherwise, <see langword="false"/> (not recommended).
        /// </param>
        protected SafeSyncHandle(IntPtr preexistingHandle, bool ownsHandle = true) : base(preexistingHandle, ownsHandle) { }

        /// <summary>Performs an implicit conversion from <see cref="SafeSyncHandle"/> to <see cref="SafeWaitHandle"/>.</summary>
        /// <param name="h">The SafeSyncHandle instance.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator SafeWaitHandle(SafeSyncHandle h) => new(h.handle, false);
    }

    /// <summary>Provides a <see cref="SafeHandle"/> to a handle that releases a created HANDLE instance at disposal using CloseHandle.</summary>
    public abstract class SafeKernelHandle : SafeHANDLE, IKernelHandle
    {
        /// <summary>Initializes a new instance of the <see cref="SafeSyncHandle"/> class.</summary>
        protected SafeKernelHandle() : base() { }

        /// <summary>Initializes a new instance of the <see cref="SafeHANDLE"/> class and assigns an existing handle.</summary>
        /// <param name="preexistingHandle">An <see cref="IntPtr"/> object that represents the pre-existing handle to use.</param>
        /// <param name="ownsHandle">
        /// <see langword="true"/> to reliably release the handle during the finalization phase; otherwise, <see langword="false"/> (not recommended).
        /// </param>
        protected SafeKernelHandle(IntPtr preexistingHandle, bool ownsHandle = true) : base(preexistingHandle, ownsHandle) { }

        /// <inheritdoc/>
        protected override bool InternalReleaseHandle() => CloseHandle(handle);
    }

    /// <summary>Signals that a structure or class holds a handle to a synchronization object.</summary>
    public interface ISyncHandle : IKernelHandle { }

    public interface IHandle
    {
        /// <summary>Returns the value of the handle field.</summary>
        /// <returns>An IntPtr representing the value of the handle field.</returns>
        IntPtr DangerousGetHandle();
    }

    /// <summary>Signals that a structure or class holds a handle to a synchronization object.</summary>
    public interface IKernelHandle : IHandle { }

    /// <summary>Base class for all native handles.</summary>
    /// <seealso cref="Microsoft.Win32.SafeHandles.SafeHandleZeroOrMinusOneIsInvalid"/>
    /// <seealso cref="System.IEquatable{T}"/>
    /// <seealso cref="IHandle"/>
    [DebuggerDisplay("{handle}")]
    public abstract class SafeHANDLE : SafeHandleZeroOrMinusOneIsInvalid, IEquatable<SafeHANDLE>, IHandle
    {
        /// <summary>Initializes a new instance of the <see cref="SafeHANDLE"/> class.</summary>
        public SafeHANDLE() : base(true)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="SafeHANDLE"/> class and assigns an existing handle.</summary>
        /// <param name="preexistingHandle">An <see cref="IntPtr"/> object that represents the pre-existing handle to use.</param>
        /// <param name="ownsHandle">
        /// <see langword="true"/> to reliably release the handle during the finalization phase; otherwise, <see langword="false"/> (not recommended).
        /// </param>
        protected SafeHANDLE(IntPtr preexistingHandle, bool ownsHandle = true) : base(ownsHandle) => SetHandle(preexistingHandle);

        /// <summary>Gets a value indicating whether this instance is null.</summary>
        /// <value><c>true</c> if this instance is null; otherwise, <c>false</c>.</value>
        public bool IsNull => handle == IntPtr.Zero;

        /// <summary>Implements the operator !=.</summary>
        /// <param name="h1">The first handle.</param>
        /// <param name="h2">The second handle.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(SafeHANDLE h1, SafeHANDLE h2) => !(h1 == h2);

        /// <summary>Implements the operator ==.</summary>
        /// <param name="h1">The first handle.</param>
        /// <param name="h2">The second handle.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(SafeHANDLE h1, SafeHANDLE h2) => h1?.Equals(h2) ?? h2 is null;

        /// <summary>Determines whether the specified <see cref="SafeHANDLE"/>, is equal to this instance.</summary>
        /// <param name="other">The <see cref="SafeHANDLE"/> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="SafeHANDLE"/> is equal to this instance; otherwise, <c>false</c>.</returns>
        public bool Equals(SafeHANDLE other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return handle == other.handle && IsClosed == other.IsClosed;
        }

        /// <summary>Determines whether the specified <see cref="System.Object"/>, is equal to this instance.</summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case IHandle ih:
                    return handle.Equals(ih.DangerousGetHandle());
                case SafeHandle sh:
                    return handle.Equals(sh.DangerousGetHandle());
                case IntPtr p:
                    return handle.Equals(p);
                default:
                    return base.Equals(obj);
            }
        }

        /// <summary>Returns a hash code for this instance.</summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>
        /// Internal method that actually releases the handle. This is called by <see cref="ReleaseHandle"/> for valid handles and afterwards
        /// zeros the handle.
        /// </summary>
        /// <returns><c>true</c> to indicate successful release of the handle; <c>false</c> otherwise.</returns>
        protected abstract bool InternalReleaseHandle();

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        protected override bool ReleaseHandle()
        {
            if (IsInvalid) return true;
            if (!InternalReleaseHandle()) return false;
            handle = IntPtr.Zero;
            return true;
        }
    }
}