using System.Text;
using static System.Buffer;
using static System.Convert;

namespace RegistryManager.Extensions;

internal static class EncodingExtensions
{
    extension(Encoding encoding)
    {
        public string? EncodeBase64(string? text) =>
            text == null
                ? null
                : ToBase64String(encoding.GetBytes(text));

        public string? DecodeBase64(string? encodedText) =>
            encodedText == null
                ? null
                : encoding.GetString(FromBase64String(encodedText));
    }

    public static byte[] DecodeToBytes(this string str)
    {
        var even = str[0] == '0';
        var bytes = new byte[((str.Length - 1) * sizeof(char)) + (even ? 0 : -1)];
        var chars = str.ToCharArray();
        BlockCopy(chars, 2, bytes, 0, bytes.Length);

        return bytes;
    }

    public static string EncodeToString(this byte[] bytes)
    {
        var even = bytes.Length % 2 == 0;
        var chars = new char[1 + (bytes.Length / sizeof(char)) + (even ? 0 : 1)];
        chars[0] = even ? '0' : '1';
        BlockCopy(bytes, 0, chars, 2, bytes.Length);

        return new string(chars);
    }
}
