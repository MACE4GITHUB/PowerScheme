using System.Text;
using static System.Convert;

namespace RegistryManager.Extensions;

internal static class EncodingExtensions
{
    public static string EncodeBase64(this Encoding encoding, string text) =>
        text == null
            ? null
            : ToBase64String(encoding.GetBytes(text));

    public static string DecodeBase64(this Encoding encoding, string encodedText) =>
        encodedText == null
            ? null
            : encoding.GetString(FromBase64String(encodedText));

    public static byte[] DecodeToBytes(this string str)
    {
        var even = str[0] == '0';
        var bytes = new byte[(str.Length - 1) * sizeof(char) + (even ? 0 : -1)];
        var chars = str.ToCharArray();
        System.Buffer.BlockCopy(chars, 2, bytes, 0, bytes.Length);

        return bytes;
    }

    public static string EncodeToString(this byte[] bytes)
    {
        var even = bytes.Length % 2 == 0;
        var chars = new char[1 + bytes.Length / sizeof(char) + (even ? 0 : 1)];
        chars[0] = even ? '0' : '1';
        System.Buffer.BlockCopy(bytes, 0, chars, 2, bytes.Length);

        return new string(chars);
    }
}