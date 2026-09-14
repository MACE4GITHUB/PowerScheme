<#
.SYNOPSIS
    Builds the PowerScheme release artifacts.

.DESCRIPTION
    Conversion of the old CreateExe.bat (+ the zip step from CreateZip.bat, as
    the removed CreateRelease.bat did) into one PowerShell release script:

      1. Rebuilds RegWriter, RunAs, Updater and PowerScheme
         (Release / net48 / win-x64).
      2. ILRepacks the three helper exes into src\PowerScheme\Resources\.
      3. ILRepacks the app and all its libraries into install\PowerScheme.exe
         (single-file output).
      4. Signs install\PowerScheme.exe with the BulovaDmitriy certificate
         (signtool, /fd SHA256, DigiCert timestamp; same toolchain as
         D:\Lic\Sign\SignPowerScheme.ps1).
      5. Compresses the merged exe into install\PowerScheme.zip.
      6. Compiles install\PowerSchemeSetup.exe with Inno Setup 6
         (src\Setup\PowerScheme.iss, which reads the app version out of the
         merged exe itself at compile time).
      7. Signs the installer too (mode B: the Updater verifies the downloaded
         PowerSchemeSetup.exe via WinVerifyTrust + pinned thumbprint, so it
         needs the same Authenticode signature).

    Fails (non-zero exit code) if any step fails, so it is CI-friendly. Pass
    -NoPause to avoid the interactive "Press Enter" at the end.

.PARAMETER Configuration
    MSBuild configuration. Default: Release.

.PARAMETER MsBuildPath
    Full path to MSBuild.exe. Defaults to the standard VS 2026 Community
    location; override if your VS edition/version differs (or use vswhere to
    auto-detect).

.PARAMETER InnoSetupPath
    Full path to ISCC.exe (Inno Setup 6). If omitted, the per-machine and the
    per-user ("%LOCALAPPDATA%\Programs\Inno Setup 6") locations are tried.

.PARAMETER PfxPath
    Path to the code-signing certificate (.pfx). Resolved in this order:
    -PfxPath argument, then the POWERSCHEME_PFX_PATH environment variable,
    then the 'POWERSCHEME_PFX_PATH' secret in Windows Credential Manager
    (Secrets.psm1 `Set-Secret`).

.PARAMETER PfxPassword
    .pfx password. Resolved in this order: -PfxPassword argument, then the
    POWERSCHEME_PFX_PASSWORD environment variable, then the
    'POWERSCHEME_PFX_PASSWORD' secret in Windows Credential Manager
    (Secrets.psm1 `Set-Secret`). The secret never has to live in this script
    or in git; an empty value aborts the signing step with a clear error.

.PARAMETER SignToolPath
    Full path to signtool.exe. If omitted, the standard Windows Kits 10 path is
    tried, then a recursive search of the installed Windows Kits.

.PARAMETER TimestampUrl
    RFC 3161 timestamp server. Default: http://timestamp.digicert.com.

.PARAMETER NoPause
    Do not wait for a key press before exiting. Useful for CI.
#>

param(
    [string]$Configuration = 'Release',
    [string]$MsBuildPath = 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe',
    [string]$InnoSetupPath = '',
    [string]$PfxPath = $env:POWERSCHEME_PFX_PATH,
    [string]$PfxPassword = $env:POWERSCHEME_PFX_PASSWORD,
    [string]$SignToolPath = '',
    [string]$TimestampUrl = 'http://timestamp.digicert.com',
    [switch]$NoPause
)

$ErrorActionPreference = 'Stop'

# Resolve the signing secrets. Precedence: explicit -PfxPath/-PfxPassword >
# environment variable > Windows Credential Manager (Secrets.psm1).
Import-Module (Join-Path $PSScriptRoot 'Secrets.psm1') -Force -Verbose

if ([string]::IsNullOrEmpty($PfxPath)) {
    $PfxPath = Get-Secret -Name 'POWERSCHEME_PFX_PATH'
}
if ([string]::IsNullOrEmpty($PfxPassword)) {
    $PfxPassword = Get-Secret -Name 'POWERSCHEME_PFX_PASSWORD'
}

# All paths are resolved against the script location, so it works from any cwd.
$RepoRoot   = $PSScriptRoot
$Framework  = 'net48'
$Rid        = 'win-x64'
$BinSubDir  = "bin\$Configuration\$Framework\$Rid"
$InstallDir = Join-Path $RepoRoot 'install'
$Resources  = Join-Path $RepoRoot 'src\PowerScheme\Resources'
$ILRepack   = Join-Path $RepoRoot 'ILRepack.exe'

# Dependency DLLs merged into each ILRepack output (leaf names within the
# project's bin\<Config>\net48\win-x64 folder).
$RegWriterDlls = @(
    'Common.Paths.dll',
    'RegistryManager.Api.dll',
    'RegistryManager.Common.dll',
    'RegistryManager.Extensions.dll',
    'RegistryManager.Savers.dll'
)
$RunAsDlls = @(
    'RunAs.Common.dll'
)
$UpdaterDlls = @(
    'Updater.Common.dll',
    'Logger.dll',
    'Microsoft.Bcl.AsyncInterfaces.dll',
    'System.Buffers.dll',
    'System.IO.Pipelines.dll',
    'System.Memory.dll',
    'System.Numerics.Vectors.dll',
    'System.Runtime.CompilerServices.Unsafe.dll',
    'System.Text.Encodings.Web.dll',
    'System.Text.Json.dll',
    'System.Threading.Tasks.Extensions.dll',
    'System.ValueTuple.dll'
)
$AppDlls = @(
    'Common.dll',
    'Common.Paths.dll',
    'Languages.dll',
    'MessageForm.dll',
    'Microsoft.Bcl.AsyncInterfaces.dll',
    'Microsoft.Extensions.DependencyInjection.dll',
    'Microsoft.Extensions.DependencyInjection.Abstractions.dll',
    'PowerManagerAPI.dll',
    'PowerSchemeServiceAPI.dll',
    'PowerScheme.Addins.IdleMonitoring.dll',
    'RegistryManager.Api.dll',
    'RegistryManager.Common.dll',
    'RegistryManager.dll',
    'RegistryManager.Dpi.dll',
    'RegistryManager.Executor.dll',
    'RegistryManager.Extensions.dll',
    'RegistryManager.Savers.dll',
    'RunAs.Common.dll',
    'System.Buffers.dll',
    'System.IO.Pipelines.dll',
    'System.Memory.dll',
    'System.Numerics.Vectors.dll',
    'System.Runtime.CompilerServices.Unsafe.dll',
    'System.Text.Encodings.Web.dll',
    'System.Text.Json.dll',
    'System.Threading.Tasks.Extensions.dll',
    'System.ValueTuple.dll',
    'Updater.Common.dll'
)

function Write-Header([string]$Title) {
    Write-Host ''
    Write-Host ('=' * 40)
    Write-Host $Title
    Write-Host ('=' * 40)
}

function Invoke-Native {
    param([string]$FilePath, [string[]]$Arguments)
    & $FilePath $Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "'$FilePath' failed with exit code $LASTEXITCODE."
    }
}

function Build-Project([string]$ProjectPath) {
    Write-Header "Build $([IO.Path]::GetFileNameWithoutExtension($ProjectPath)) project"
    Invoke-Native $MsBuildPath @('/t:Rebuild', "/p:Configuration=$Configuration", '/v:m', $ProjectPath)
}

function Merge-Exe {
    param(
        [string]$SourceProject, # folder name under src\, e.g. 'RegWriter'
        [string]$ExeName,       # main exe to merge, e.g. 'RegWriter.exe'
        [string[]]$Dlls,        # dependency DLLs to merge
        [string]$Output         # full output path of the merged exe
    )
    $srcDir = Join-Path $RepoRoot "src\$SourceProject\$BinSubDir"
    $inputs = @(Join-Path $srcDir $ExeName)
    $inputs += $Dlls | ForEach-Object { Join-Path $srcDir $_ }
    $args = @("/out:$Output") + $inputs + @('/skipconfig', '/ndebug', '/parallel')
    Invoke-Native $ILRepack $args
}

function Find-ISCC {
    if ($InnoSetupPath -and (Test-Path $InnoSetupPath)) {
        return $InnoSetupPath
    }
    $candidates = @(
        (Join-Path ${env:ProgramFiles(x86)} 'Inno Setup 6\ISCC.exe'),
        (Join-Path $env:ProgramFiles 'Inno Setup 6\ISCC.exe'),
        (Join-Path $env:LOCALAPPDATA 'Programs\Inno Setup 6\ISCC.exe')
    )
    foreach ($candidate in $candidates) {
        if (Test-Path $candidate) { return $candidate }
    }
    return $null
}

function Find-SignTool {
    if ($SignToolPath -and (Test-Path $SignToolPath)) {
        return $SignToolPath
    }
    $known = 'C:\Program Files (x86)\Windows Kits\10\bin\10.0.26100.0\x64\signtool.exe'
    if (Test-Path $known) { return $known }
    $found = Get-ChildItem -Path 'C:\Program Files (x86)\Windows Kits\10\bin' -Filter 'signtool.exe' -Recurse -ErrorAction SilentlyContinue |
        Where-Object { $_.FullName -match '\\x64\\signtool\.exe$' } |
        Sort-Object FullName -Descending | Select-Object -First 1
    if ($found) { return $found.FullName }
    return $null
}

function Sign-File {
    param(
        [Parameter(Mandatory)]
        [string]$FilePath,
        [Parameter(Mandatory)]
        [string]$SignTool
    )
    # Same toolchain as D:\Lic\Sign\SignPowerScheme.ps1: /fd SHA256 + RFC 3161
    # DigiCert timestamp. Every release artifact that the Updater is allowed to
    # download must carry this signature (pinned thumbprint in SignatureVerifier).
    & $SignTool sign /fd SHA256 /f $PfxPath /p $PfxPassword /tr $TimestampUrl /td SHA256 $FilePath
    if ($LASTEXITCODE -ne 0) {
        throw "signtool failed with exit code ${LASTEXITCODE}: $FilePath"
    }
    Write-Host "    Signed: $FilePath"
}

try {
    if (-not (Test-Path $MsBuildPath)) {
        throw "MSBuild.exe not found at '$MsBuildPath'. Pass -MsBuildPath or install VS."
    }
    Set-Location $RepoRoot

    # --- 1-2. Helpers (built first so the main project picks them up as resources) ---
    Build-Project (Join-Path $RepoRoot 'src\RegWriter\RegWriter.csproj')
    Merge-Exe 'RegWriter' 'RegWriter.exe' $RegWriterDlls (Join-Path $Resources 'RegWriter.exe')

    Build-Project (Join-Path $RepoRoot 'src\RunAs\RunAs.csproj')
    Merge-Exe 'RunAs' 'RunAs.exe' $RunAsDlls (Join-Path $Resources 'RunAs.exe')

    Build-Project (Join-Path $RepoRoot 'src\Updater\Updater.csproj')
    Merge-Exe 'Updater' 'Updater.exe' $UpdaterDlls (Join-Path $Resources 'Updater.exe')

    # --- 3. Main application (single-file) ---
    Build-Project (Join-Path $RepoRoot 'src\PowerScheme\PowerScheme.csproj')
    Write-Header 'Create PowerScheme installation'
    Merge-Exe 'PowerScheme' 'PowerScheme.exe' $AppDlls (Join-Path $InstallDir 'PowerScheme.exe')

    # --- 4. Sign the merged exe (same cert/password as D:\Lic\Sign\SignPowerScheme.ps1) ---
    Write-Header 'Sign PowerScheme.exe'
    $signTool = Find-SignTool
    if ($signTool) {
        if ([string]::IsNullOrEmpty($PfxPath)) {
            throw "PFX path is empty. Set the POWERSCHEME_PFX_PATH environment variable or pass -PfxPath."
        }
        if ([string]::IsNullOrEmpty($PfxPassword)) {
            throw "PFX password is empty. Set the POWERSCHEME_PFX_PASSWORD environment variable or pass -PfxPassword."
        }
        if (Test-Path $PfxPath) {
            Sign-File (Join-Path $InstallDir 'PowerScheme.exe') $signTool
        }
        else {
            Write-Warning "PFX not found at '$PfxPath'. Pass -PfxPath; PowerScheme.exe was not signed."
        }
    }
    else {
        Write-Warning 'signtool.exe not found. Pass -SignToolPath or install the Windows SDK; PowerScheme.exe was not signed.'
    }

    # --- 5. Zip archive ---
    Write-Header 'Create PowerScheme zip archive'
    $zipPath = Join-Path $InstallDir 'PowerScheme.zip'
    if (Test-Path $zipPath) { Remove-Item $zipPath -Force }
    Compress-Archive -Path (Join-Path $InstallDir 'PowerScheme.exe') -DestinationPath $zipPath

    # --- 6. Inno Setup installer ---
    Write-Header 'Create PowerScheme Inno Setup installer'
    $iscc = Find-ISCC
    if ($iscc) {
        $appExe = Join-Path $InstallDir 'PowerScheme.exe'
        $appVersion = if (Test-Path $appExe) { (Get-Item $appExe).VersionInfo.FileVersion } else { '0.0.0.0' }
        Write-Host "    Version: $appVersion"
        Invoke-Native $iscc @('/Q', (Join-Path $RepoRoot 'src\Setup\PowerScheme.iss'))
    }
    else {
        Write-Warning "Inno Setup 6 compiler not found. Pass -InnoSetupPath or install Inno Setup 6; PowerSchemeSetup.exe was not built."
    }

    # --- 7. Sign the installer too. Mode B ships updates as PowerSchemeSetup.exe,
    #      and the Updater verifies every downloaded asset via WinVerifyTrust +
    #      pinned thumbprint, so the installer must carry the same signature. ---
    Write-Header 'Sign PowerSchemeSetup.exe'
    if ($signTool) {
        $setupExe = Join-Path $InstallDir 'PowerSchemeSetup.exe'
        if (Test-Path $setupExe) {
            Sign-File $setupExe $signTool
        }
        else {
            Write-Warning 'PowerSchemeSetup.exe not found; it was not signed.'
        }
    }
    else {
        Write-Warning 'signtool.exe not found; PowerSchemeSetup.exe was not signed.'
    }

    Write-Host ''
    Write-Host 'All operations completed.'
}
catch {
    Write-Host ''
    Write-Host "ERROR: $($_.Exception.Message)" -ForegroundColor Red
    if (-not $NoPause) { Read-Host 'Press Enter to exit' }
    exit 1
}

if (-not $NoPause) { Read-Host 'Press Enter to exit' }