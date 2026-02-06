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
set APP_DIR=.\src\PowerScheme\bin\%CONFIG%\%FRAMEWORK%\%RID%
set MSBUILD="C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
REM ===============================================

call :PrintHeader "Build RegWriter project"
call :BuildProject ".\src\RegWriter\RegWriter.csproj"
copy .\src\RegWriter\bin\%CONFIG%\%FRAMEWORK%\%RID%\%REGWRITER_EXE% %RESOURCES_DIR%

call :PrintHeader "Build RunAs project"
call :BuildProject ".\src\RunAs\RunAs.csproj"
copy .\src\RunAs\bin\%CONFIG%\%FRAMEWORK%\%RID%\%RUNAS_EXE% %RESOURCES_DIR%

call :PrintHeader "Build Updater project"
call :BuildProject ".\src\Updater\Updater.csproj"
copy .\src\Updater\bin\%CONFIG%\%FRAMEWORK%\%RID%\%UPDATER_EXE% %RESOURCES_DIR%

call :PrintHeader "Build PowerScheme project"
call :BuildProject ".\src\PowerScheme\PowerScheme.csproj"

call :PrintHeader "Create PowerScheme installation"
ILRepack.exe ^
 /out:install\PowerScheme.exe ^
 %APP_DIR%\PowerScheme.exe ^
 %APP_DIR%\Microsoft.Extensions.DependencyInjection.dll ^
 %APP_DIR%\PowerManagerAPI.dll ^
 %APP_DIR%\PowerSchemeServiceAPI.dll ^
 %APP_DIR%\RegistryManager.dll ^
 %APP_DIR%\RunAs.Common.dll ^
 %APP_DIR%\System.Runtime.CompilerServices.Unsafe.dll ^
 %APP_DIR%\System.Threading.Tasks.Extensions.dll ^
 %APP_DIR%\Common.dll ^
 %APP_DIR%\Languages.dll ^
 %APP_DIR%\MessageForm.dll ^
 %APP_DIR%\Microsoft.Bcl.AsyncInterfaces.dll ^
 %APP_DIR%\Microsoft.Extensions.DependencyInjection.Abstractions.dll ^
 /skipconfig ^
 /ndebug

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
exit /b
