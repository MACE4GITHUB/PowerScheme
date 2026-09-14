using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;

namespace Updater.Common;

/// <summary>
/// Verifies Authenticode signature of a PE file against a pinned public key.
/// Uses WinVerifyTrust for hash verification and thumbprint pinning for key verification.
/// </summary>
public sealed class SignatureVerifier
{
    // Pinned public key: BulovaDmitriy.cer (self-signed code-signing cert, RSA-2048/SHA-256)
    // Thumbprint: 8C330C9487643494BBF1AB3EA4828C11C466BE74
    // To regenerate after cert renewal:
    //   openssl base64 -in BulovaDmitriy.cer -A | tr -d '\n'
    // Then replace the constant below and update the thumbprint.
    private const string PinnedCertBase64 =
        "MIIDbjCCAlagAwIBAgIQfeJ3gH6+UZ5G6Bdyad+JBzANBgkqhkiG9w0BAQsFADBPMQswCQYDVQQGEwJCWTEXMBUGA1UEAwwOQnVsb3ZhIERtaXRyaXkxJzAlBgkqhkiG9w0BCQEWGGRtaXRyaXkuYnVsb3ZhQGdtYWlsLmNvbTAeFw0yNjA5MTAxMjU2MTla" +
        "Fw0zNjA5MTAxMzA2MTlaME8xCzAJBgNVBAYTAkJZMRcwFQYDVQQDDA5CdWxvdmEgRG1pdHJpeTEnMCUGCSqGSIb3DQEJARYYZG1pdHJpeS5idWxvdmFAZ21haWwuY29tMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAsEMpag+eaaqBfGRaiFx4ujkT" +
        "3Z1dqKJluetYY9vw/ya80eT6RSX4tgl/k5VdB8kMpA9ofEuhH6/oety8q87mpMMiIuWAgz3XQwJNfVXzqU+sDNSldIwLBGel3QSoZ6rflByRCLZFGotpEXG34/s/v2jbXKFZ0sY7HSNVMTMUCJ3HCNLhu0UkHNIYoCg8ZQqzSXd9tict44IfokbsZytJrccZ" +
        "A4FvYHMNBjPMaMIHOyJ9NSPVjAcvOCIoGmQwr2QlQBYr1wV1JXUnLWeuhHAAcX3JPkyf/7My/3Yrzkeiyg9Y72bprP38ESgO3zdEPwFCnWNkakDVHjy1G787RN3x8QIDAQABo0YwRDAOBgNVHQ8BAf8EBAMCB4AwEwYDVR0lBAwwCgYIKwYBBQUHAwMw" +
        "HQYDVR0OBBYEFBNdzKcPwR2yB4pESZ6TaRcGOTp+MA0GCSqGSIb3DQEBCwUAA4IBAQCsb61e8GIkeJQ91VwZyFNSUH3AW2mVkUCbN3Q2smVGzrHHIuKAxdHaqpZsq/Qa+J2nwWXvhR7F5HgnwTg0kVnkQJuKPE465Cv97C7+3kmEZ6qm4ZfgfUMdbPUNF4kV" +
        "skQWb8UVDCFRUOytI6NzAHs6mp3vzLx3oVpQgxjAHlXfHt7hAZmVFnzUyISI0B7a0Abt7Wj2elrIMPAEzg5KV+ALDukvCEyt5paJ0kbbtGFzb2o+SCk/OM21CFOq/L7sOvW1QeIIjq6f4WD/xAZyQtO+zI38tbFfw9Y3Z1dPSSfQF1w9R0YckQuF8ex20QIUSNs" +
        "HmFYCS1c/0W3xqFo+QLlC";

    // Derived at runtime from PinnedCertBase64 so there is a single source of truth.
    private static readonly string PinnedThumbprint =
        new X509Certificate2(Convert.FromBase64String(PinnedCertBase64)).Thumbprint;

    private const ushort WinCertificateTypePkcsSignedData = 0x0002;

    // WinVerifyTrust error codes
    private const int ErrorSuccess = 0;
    private const int CertEUntrustedRoot = unchecked((int)0x800B0109);

    // WinVerifyTrust action
    private static readonly Guid WinTrustActionGenericVerifyV2 =
        new("00AAC56B-CD44-11d0-8CC2-00C04FC295EE");

    /// <summary>
    /// Verifies the Authenticode signature of the specified PE file.
    /// Returns true only if:
    /// 1. The file hash matches the embedded signature (WinVerifyTrust)
    /// 2. The signing certificate matches our pinned key (thumbprint check)
    /// </summary>
    public bool Verify(string filePath)
    {
        try
        {
            if (!VerifyFileHash(filePath))
            {
                return false;
            }

            return VerifySignerThumbprint(filePath);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Uses WinVerifyTrust to verify that the PE file hash matches the Authenticode signature.
    /// Accepts ERROR_SUCCESS (trusted) and CERT_E_UNTRUSTED_ROOT (valid sig, untrusted self-signed cert).
    /// </summary>
    private static bool VerifyFileHash(string filePath)
    {
        var fullPath = Path.GetFullPath(filePath);

        var fileInfo = new WinTrustFileInfo
        {
            cbStruct = Marshal.SizeOf(typeof(WinTrustFileInfo)),
            pcwszFilePath = fullPath,
            hFile = IntPtr.Zero,
            pgKnownSubject = IntPtr.Zero
        };

        var pFileInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(WinTrustFileInfo)));
        try
        {
            Marshal.StructureToPtr(fileInfo, pFileInfo, false);

            var trustData = new WinTrustData
            {
                cbStruct = Marshal.SizeOf(typeof(WinTrustData)),
                dwUIChoice = 2, // WTD_UI_NONE
                fdwRevocationChecks = 0, // WTD_REVOKE_NONE
                dwUnionChoice = 1, // WTD_CHOICE_FILE
                pFile = pFileInfo,
                dwStateAction = 1, // WTD_STATEACTION_VERIFY
            };

            var pTrustData = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(WinTrustData)));
            try
            {
                Marshal.StructureToPtr(trustData, pTrustData, false);

                var actionId = WinTrustActionGenericVerifyV2;
                var hr = WinVerifyTrust(IntPtr.Zero, ref actionId, pTrustData);

                // Close state data
                trustData.dwStateAction = 2; // WTD_STATEACTION_CLOSE
                Marshal.StructureToPtr(trustData, pTrustData, false);
                WinVerifyTrust(IntPtr.Zero, ref actionId, pTrustData);

                // Accept: signature is valid (hash matches), cert may be untrusted (self-signed)
                return hr == ErrorSuccess || hr == CertEUntrustedRoot;
            }
            finally
            {
                Marshal.FreeHGlobal(pTrustData);
            }
        }
        finally
        {
            Marshal.FreeHGlobal(pFileInfo);
        }
    }

    /// <summary>
    /// Extracts the PKCS#7 from the PE certificate table.
    /// Cryptographically verifies the signature and pins the signer's thumbprint.
    /// </summary>
    private static bool VerifySignerThumbprint(string filePath)
    {
        var pkcs7 = ExtractPkcs7FromPe(filePath);
        if (pkcs7 == null)
        {
            return false;
        }

        var signedCms = new SignedCms();
        signedCms.Decode(pkcs7);

        // Cryptographically verify the PKCS#7 signature with the signer's own certificate.
        // This ensures the embedded certificate is self-consistent with the signature.
        signedCms.CheckSignature(verifySignatureOnly: true);

        if (signedCms.SignerInfos.Count == 0)
        {
            return false;
        }

        var signerCert = signedCms.SignerInfos[0].Certificate;
        return string.Equals(signerCert.Thumbprint, PinnedThumbprint, StringComparison.OrdinalIgnoreCase);
    }

    private static byte[]? ExtractPkcs7FromPe(string filePath)
    {
        var peData = File.ReadAllBytes(filePath);

        // MZ header
        if (peData.Length < 0x40 || peData[0] != 0x4D || peData[1] != 0x5A)
        {
            return null;
        }

        var e_lfanew = BitConverter.ToInt32(peData, 0x3C);
        if (e_lfanew < 0 || e_lfanew + 4 > peData.Length)
        {
            return null;
        }

        // PE signature
        if (BitConverter.ToUInt32(peData, e_lfanew) != 0x00004550) // "PE\0\0"
        {
            return null;
        }

        var optStart = e_lfanew + 24;
        if (optStart + 2 > peData.Length)
        {
            return null;
        }

        var magic = BitConverter.ToUInt16(peData, optStart);

        int certTableOffset;
        if (magic == 0x20B) // PE32+
        {
            certTableOffset = optStart + 112 + 4 * 8;
        }
        else if (magic == 0x10B) // PE32
        {
            certTableOffset = optStart + 96 + 4 * 8;
        }
        else
        {
            return null;
        }

        if (certTableOffset + 8 > peData.Length)
        {
            return null;
        }

        var certRva = BitConverter.ToInt32(peData, certTableOffset); // Actually file offset
        var certSize = BitConverter.ToInt32(peData, certTableOffset + 4);

        if (certSize == 0 || certRva == 0 || certRva + 8 > peData.Length)
        {
            return null; // No signature
        }

        // WIN_CERTIFICATE header
        var dwLength = BitConverter.ToInt32(peData, certRva);
        var wCertificateType = BitConverter.ToUInt16(peData, certRva + 6);

        if (wCertificateType != WinCertificateTypePkcsSignedData)
        {
            return null;
        }

        if (dwLength < 8 || certRva + dwLength > peData.Length)
        {
            return null;
        }

        var pkcs7Length = dwLength - 8;
        var pkcs7 = new byte[pkcs7Length];
        Array.Copy(peData, certRva + 8, pkcs7, 0, pkcs7Length);

        return pkcs7;
    }

    #region P/Invoke

    [DllImport("wintrust.dll", SetLastError = true)]
    private static extern int WinVerifyTrust(
        IntPtr hwnd,
        ref Guid pgActionID,
        IntPtr pWVTData);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct WinTrustFileInfo
    {
        public int cbStruct;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string pcwszFilePath;
        public IntPtr hFile;
        public IntPtr pgKnownSubject;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct WinTrustData
    {
        public int cbStruct;
        public IntPtr pPolicyCallbackData;
        public IntPtr pSIPClientData;
        public int dwUIChoice;
        public int fdwRevocationChecks;
        public int dwUnionChoice;
        public IntPtr pFile;
        public int dwStateAction;
        public IntPtr hWVTStateData;
        public IntPtr pwszURLReference;
        public int dwProvFlags;
        public int dwUIContext;
        public IntPtr pSignatureSettings;
    }

    #endregion
}
