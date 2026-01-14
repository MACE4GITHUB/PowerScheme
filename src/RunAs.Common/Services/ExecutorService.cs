using System;
using System.Diagnostics;
using System.IO;
using System.Resources;
using RunAs.Common.Utils;

namespace RunAs.Common.Services;

public class ExecutorService
{
    private const string EXE_EXTENSION = ".exe";

    public ExecutorService()
    { }

    public ExecutorService(string name, string arguments, ResourceManager resourceManager)
    {
        Name = name;
        Arguments = arguments;
        ResourceManager = resourceManager;
    }

    public string Name { get; set; } = string.Empty;

    public string FullName
        => Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName)!, $"{Name}{EXE_EXTENSION}");

    public string Arguments { get; set; } = string.Empty;

    public ResourceManager? ResourceManager { get; set; }

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
                RemoveIfExists();
            }
        }
    }

    public virtual void RemoveIfExists()
    {
        if (File.Exists(FullName))
        {
            File.Delete(FullName);
        }
    }

    private byte[] Executor
        => (byte[])(ResourceManager?.GetObject(Name)
                    ?? throw new ArgumentNullException(nameof(Name)));
}