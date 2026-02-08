using System;
using System.IO;
using RegistryManager.Extensions;
using RegistryManager.Model;

namespace RegistryManager;

public sealed class RegistrySaver(
    string operation,
    string fileName) :
    IDisposable
{
    private const string REG_EXTENSION = "";

    public RegistrySaver(RegistryAdminAction registryAdminAction, string fileName) :
        this(registryAdminAction.ToString(), fileName)
    { }

    public RegistrySaver(RegistryAdminAction registryAdminAction, Guid guid) :
        this(registryAdminAction.ToString(), guid.ToString())
    { }

    public string FileName { get; } = fileName;

    public string FileFullName
        => Path.Combine(Paths.ApplicationPath, FileName + REG_EXTENSION);

    public string Operation { get; } = operation;

    public RegistryParam RegistryParam { get; set; }

    public void SaveToRegistry()
    {
        try
        {
            var settings = File.ReadAllText(FileFullName);

            var decodeText =
                System.Text.Encoding.UTF8.DecodeBase64(settings);

            if (decodeText == null)
            {
                return;
            }

            var registryParam = RegistryParam.FromString(decodeText);

            var currentOperation = (RegistryAdminAction)Enum.Parse(typeof(RegistryAdminAction), Operation, true);
            switch (currentOperation)
            {
                case RegistryAdminAction.Set:
                    Registry.SaveSetting(registryParam);
                    break;
                case RegistryAdminAction.Delete:
                    Registry.DeleteSetting(registryParam);
                    break;
                default:
                    throw new InvalidOperationException(nameof(currentOperation));
            }
        }
        catch (Exception)
        {
            // nothing
        }
    }

    public string Arguments => $"""
                                "{Operation}" "{FileName}"
                                """;

    public void SaveToStore()
    {
        var encodeText =
            System.Text.Encoding.UTF8.EncodeBase64(RegistryParam.ToString());
        File.WriteAllText(FileFullName, encodeText);
        File.SetAttributes(FileFullName, FileAttributes.Hidden);
    }

    public void Dispose()
    {
        if (File.Exists(FileFullName))
        {
            File.Delete(FileFullName);
        }
    }
}
