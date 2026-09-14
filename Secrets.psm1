# The P/Invoke surface lives in its own namespace so it never clashes with the
# CmdLets below. -TypeDefinition (not -MemberDefinition) is required: the source
# contains full type declarations (enum + struct + class) and top-level using
# directives, which -MemberDefinition would nest inside the generated class.
# The guard skips recompilation when the module is reimported in the same session.
if (-not ('Win32.CredApi' -as [type])) {
    Add-Type -TypeDefinition @'
using System;
using System.Runtime.InteropServices;

namespace Win32
{
    public enum CRED_TYPE : uint
    {
        GENERIC = 1,
        DOMAIN_PASSWORD = 2,
        DOMAIN_CERTIFICATE = 3,
        DOMAIN_VISIBLE_PASSWORD = 4,
        GENERIC_CERTIFICATE = 5,
        DOMAIN_EXTENDED = 6,
        MAXIMUM = 7,
        MAXIMUM_EX = 8
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct CREDENTIAL
    {
        public uint Flags;
        public CRED_TYPE Type;
        public IntPtr TargetName;
        public IntPtr Comment;
        public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
        public uint CredentialBlobSize;
        public IntPtr CredentialBlob;
        public uint Persist;
        public uint AttributeCount;
        public IntPtr Attributes;
        public IntPtr TargetAlias;
        public IntPtr UserName;
    }

    public static class CredApi
    {
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool CredRead(
            string target,
            CRED_TYPE type,
            int reservedFlag,
            out IntPtr credentialPtr);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool CredWrite(
            ref CREDENTIAL userCredential,
            uint flags);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern void CredFree(IntPtr buffer);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool CredDelete(
            string target,
            CRED_TYPE type,
            uint flags);
    }
}
'@
}

function Set-Secret {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [string]$Name,

        [Parameter(Mandatory)]
        [string]$Value,

        [ValidateSet('LocalMachine','Enterprise','Session')]
        [string]$Persist = 'LocalMachine'
    )

    $bytes = [System.Text.Encoding]::UTF8.GetBytes($Value)

    $cred = New-Object Win32.CREDENTIAL
    $cred.Flags = 0
    $cred.Type  = [Win32.CRED_TYPE]::GENERIC
    $cred.TargetName = [System.Runtime.InteropServices.Marshal]::StringToHGlobalUni($Name)
    $cred.CredentialBlobSize = $bytes.Length
    $cred.CredentialBlob = [System.Runtime.InteropServices.Marshal]::AllocHGlobal($bytes.Length)
    [System.Runtime.InteropServices.Marshal]::Copy($bytes, 0, $cred.CredentialBlob, $bytes.Length)

    switch ($Persist) {
        'LocalMachine' { $cred.Persist = 2 }
        'Enterprise'   { $cred.Persist = 3 }
        'Session'      { $cred.Persist = 1 }
    }

    # Free the unmanaged buffers even if CredWrite throws, so a failed write
    # cannot leak the handles.
    try {
        $ok = [Win32.CredApi]::CredWrite([ref]$cred, 0)
    }
    finally {
        [System.Runtime.InteropServices.Marshal]::FreeHGlobal($cred.TargetName)
        [System.Runtime.InteropServices.Marshal]::FreeHGlobal($cred.CredentialBlob)
    }

    if (-not $ok) {
        throw "CredWrite failed. Win32Error=$([Runtime.InteropServices.Marshal]::GetLastWin32Error())"
    }
}

function Get-Secret {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [string]$Name
    )

    $ptr = [IntPtr]::Zero
    $ok = [Win32.CredApi]::CredRead($Name, [Win32.CRED_TYPE]::GENERIC, 0, [ref]$ptr)

    if (-not $ok -or $ptr -eq [IntPtr]::Zero) {
        return $null
    }

    try {
        # Marshal.PtrToStructure is unreliable for Add-Type types in Windows
        # PowerShell (it rejects them as "non-blittable"), so CREDENTIAL is
        # parsed field by field at the Win32 offsets instead.
        $is64 = [Environment]::Is64BitProcess
        $blobSizeOff = if ($is64) { 32 } else { 24 }
        $blobPtrOff  = if ($is64) { 40 } else { 28 }

        $size = [System.Runtime.InteropServices.Marshal]::ReadInt32($ptr, $blobSizeOff)
        $blob = [System.Runtime.InteropServices.Marshal]::ReadIntPtr($ptr, $blobPtrOff)
        if ($size -le 0 -or $blob -eq [IntPtr]::Zero) {
            return $null
        }

        $bytes = New-Object byte[] $size
        [System.Runtime.InteropServices.Marshal]::Copy($blob, $bytes, 0, $size)
        return [System.Text.Encoding]::UTF8.GetString($bytes)
    }
    finally {
        [Win32.CredApi]::CredFree($ptr)
    }
}

function Remove-Secret {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [string]$Name
    )

    $ok = [Win32.CredApi]::CredDelete($Name, [Win32.CRED_TYPE]::GENERIC, 0)
    if (-not $ok) {
        throw "CredDelete failed. Win32Error=$([Runtime.InteropServices.Marshal]::GetLastWin32Error())"
    }
}

function Show-Secret {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [string]$Name,

        [switch]$Reveal
    )

    $value = Get-Secret -Name $Name

    if ($null -eq $value) {
        Write-Warning "Secret '$Name' not found."
        return
    }

    $masked = if ($Reveal) {
        $value
    } else {
        if ($value.Length -le 3) {
            '*' * $value.Length
        } else {
            $value.Substring(0,3) + ('*' * ($value.Length - 3))
        }
    }

    [PSCustomObject]@{
        Name        = $Name
        Value       = $masked
        Revealed    = $Reveal.IsPresent
        Length      = $value.Length
        RetrievedAt = (Get-Date)
    } | Format-List
}

Export-ModuleMember -Function Set-Secret, Get-Secret, Remove-Secret, Show-Secret
