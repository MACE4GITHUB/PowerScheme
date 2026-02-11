@echo off
REM ===============================================
REM Build configuration (Release or Debug)
set CONFIG=Release
set FRAMEWORK=net48
set RID=win-x64
set REGWRITER_EXE=RegWriter.exe
set RUNAS_EXE=RunAs.exe
set UPDATER_EXE=Updater.exe
set RESOURCES_DIR=.\src\PowerScheme\Resources
set BIN_DIR=bin\%CONFIG%\%FRAMEWORK%\%RID%
set APP_DIR=.\src\PowerScheme\%BIN_DIR%
set REGWRITER_DIR=.\src\RegWriter\%BIN_DIR%
set RUNAS_DIR=.\src\RunAs\%BIN_DIR%
set UPDATER_DIR=.\src\Updater\%BIN_DIR%
set MSBUILD="C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
REM ===============================================

call :PrintHeader "Build RegWriter project"
call :BuildProject ".\src\RegWriter\RegWriter.csproj"
ILRepack.exe ^
 /out:%RESOURCES_DIR%\RegWriter.exe ^
 %REGWRITER_DIR%\RegWriter.exe ^
 %REGWRITER_DIR%\Common.Paths.dll ^
 %REGWRITER_DIR%\RegistryManager.Api.dll ^
 %REGWRITER_DIR%\RegistryManager.Common.dll ^
 %REGWRITER_DIR%\RegistryManager.Extensions.dll ^
 %REGWRITER_DIR%\RegistryManager.Savers.dll ^
 /skipconfig ^
 /ndebug ^
 /parallel

call :PrintHeader "Build RunAs project"
call :BuildProject ".\src\RunAs\RunAs.csproj"
ILRepack.exe ^
 /out:%RESOURCES_DIR%\RunAs.exe ^
 %RUNAS_DIR%\RunAs.exe ^
 %RUNAS_DIR%\RunAs.Common.dll ^
 /skipconfig ^
 /ndebug ^
 /parallel

call :PrintHeader "Build Updater project"
call :BuildProject ".\src\Updater\Updater.csproj"
ILRepack.exe ^
 /out:%RESOURCES_DIR%\Updater.exe ^
 %UPDATER_DIR%\Updater.exe ^
 %UPDATER_DIR%\Updater.Common.dll ^
 %UPDATER_DIR%\Logger.dll ^
 /skipconfig ^
 /ndebug ^
 /parallel

call :PrintHeader "Build PowerScheme project"
call :BuildProject ".\src\PowerScheme\PowerScheme.csproj"

call :PrintHeader "Create PowerScheme installation"
ILRepack.exe ^
 /out:install\PowerScheme.exe ^
 %APP_DIR%\PowerScheme.exe ^
 %APP_DIR%\Common.dll ^
 %APP_DIR%\Common.Paths.dll ^
 %APP_DIR%\Languages.dll ^
 %APP_DIR%\MessageForm.dll ^
 %APP_DIR%\Microsoft.Bcl.AsyncInterfaces.dll ^
 %APP_DIR%\Microsoft.Extensions.DependencyInjection.dll ^
 %APP_DIR%\Microsoft.Extensions.DependencyInjection.Abstractions.dll ^
 %APP_DIR%\PowerManagerAPI.dll ^
 %APP_DIR%\PowerSchemeServiceAPI.dll ^
 %APP_DIR%\RegistryManager.Api.dll ^
 %APP_DIR%\RegistryManager.Common.dll ^
 %APP_DIR%\RegistryManager.dll ^
 %APP_DIR%\RegistryManager.Dpi.dll ^
 %APP_DIR%\RegistryManager.Executor.dll ^
 %APP_DIR%\RegistryManager.Extensions.dll ^
 %APP_DIR%\RegistryManager.Savers.dll ^
 %APP_DIR%\RunAs.Common.dll ^
 %APP_DIR%\System.Runtime.CompilerServices.Unsafe.dll ^
 %APP_DIR%\System.Threading.Tasks.Extensions.dll ^
 %APP_DIR%\Updater.Common.dll ^
 /skipconfig ^
 /ndebug ^
 /parallel


call :PrintHeader "Create PowerScheme zip archive"
if exist ".\install\PowerScheme.zip" del ".\install\PowerScheme.zip"
powershell Compress-Archive ^
 -Path ".\install\PowerScheme.exe" ^
 -DestinationPath ".\install\PowerScheme.zip"

call :PrintHeader "All operations completed"

pause

exit /b

REM Function header definition
:PrintHeader
echo.
echo =========================================
echo %~1
echo =========================================
echo.
exit /b

REM Function Build Project
:BuildProject
"C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" %~1 ^
 /t:Rebuild ^
 /p:Configuration=%CONFIG% ^
 /v:m
 echo.
exit /b
