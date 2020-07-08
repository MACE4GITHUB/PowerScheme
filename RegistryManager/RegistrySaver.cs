using RegistryManager.Model;
using System;
using System.IO;

namespace RegistryManager
{
    public class RegistrySaver : IDisposable
    {
        private const string REG_EXTENTION = "";
        private bool disposedValue;

        public RegistrySaver(RegistryAdminAction registryAdminAction, string fileName) :
            this(registryAdminAction.ToString(), fileName)
        { }

        public RegistrySaver(RegistryAdminAction registryAdminAction, Guid guid) :
            this(registryAdminAction.ToString(), guid.ToString())
        { }

        public RegistrySaver(string operation, string fileName)
        {
            Operation = operation;
            FileName = fileName;
        }

        public string FileName { get; }

        public string FileFullName
            => Path.Combine(Paths.ApplicationPath, FileName + REG_EXTENTION);

        public string Operation { get; }

        public RegistryParam RegistryParam { get; set; }

        public void SaveToRegistry()
        {
            try
            {
                var settings = File.ReadAllText(FileFullName);

                var decodeText =
                    System.Text.Encoding.UTF8.DecodeBase64(settings);

                var registryParam = RegistryParam.FromString(decodeText);

                var operation = (RegistryAdminAction)Enum.Parse(typeof(RegistryAdminAction), Operation.ToLowerInvariant());
                switch (operation)
                {
                    case RegistryAdminAction.set:
                        Registry.SaveSetting(registryParam);
                        break;
                    case RegistryAdminAction.delete:
                        Registry.DeleteSetting(registryParam);
                        break;
                }
            }
            catch (Exception)
            {
                // nothing
            }
        }

        public string Arguments => $"\"{Operation}\" \"{FileName}\"";

        public void SaveToStore()
        {
            var encodeText =
                System.Text.Encoding.UTF8.EncodeBase64(RegistryParam.ToString());
            File.WriteAllText(FileFullName, encodeText);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue) return;
            if (disposing)
            {
                if (File.Exists(FileFullName)) File.Delete(FileFullName);
            }

            disposedValue = true;
        }

        void IDisposable.Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            ((IDisposable)this).Dispose();
        }
    }
}
