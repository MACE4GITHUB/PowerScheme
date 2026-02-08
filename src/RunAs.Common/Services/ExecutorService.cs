using System;
using System.Diagnostics;
using System.IO;
using System.Resources;
using RunAs.Common.Utils;

namespace RunAs.Common.Services;

public class ExecutorService(
    string name,
    string arguments,
    ResourceManager? resourceManager)
{
    private const string EXE_EXTENSION = ".exe";

    public string Name { get; set; } = name;

    public string FullName
        => Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName)!, $"{Name}{EXE_EXTENSION}");

    public string Arguments { get; set; } = arguments;

    public ResourceManager? ResourceManager { get; set; } = resourceManager;

    public ProcessWindowStyle ProcessWindowStyle { get; set; } = ProcessWindowStyle.Hidden;

    public bool Runas { get; set; } = true;

    public bool IsWait { get; set; } = true;

    public bool UseExecutor { get; set; } = true;

    public bool IsRemoveFile { get; set; } = true;

    public bool IsHiddenFile { get; set; } = true;

    public virtual void Execute()
    {
        if (UseExecutor)
        {
            RemoveFileIfExists();
            File.WriteAllBytes(FullName, Executor);
        }

        if (IsHiddenFile)
        {
            File.SetAttributes(FullName, FileAttributes.Hidden);
        }

        try
        {
            UacHelper.AttemptPrivilegeEscalation(
                FullName,
                Arguments,
                ProcessWindowStyle,
                Runas,
                IsWait);
        }
        finally
        {
            if (IsRemoveFile)
            {
                RemoveFileIfExists();
            }
        }
    }

    public virtual void RemoveFileIfExists()
    {
        if (File.Exists(FullName))
        {
            File.Delete(FullName);
        }
    }

    private byte[] Executor
        => (byte[])(ResourceManager?.GetObject(Name)
                    ?? throw new ArgumentException("The application resource executable file was not found."));
}
