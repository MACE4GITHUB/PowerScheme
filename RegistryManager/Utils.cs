﻿namespace RegistryManager
{
    /// <summary>
    /// Contains common registry related utility methods.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Split the given string by a string.
        /// </summary>
        /// <param name="stringToSplit">string to split</param>
        /// <param name="separator">separator as string</param>
        /// <returns>string array</returns>
        public static string[] SplitByString(this string stringToSplit, string separator)
        {
            var offset = 0;
            var index = 0;
            var offsets = new int[stringToSplit.Length + 1];

            while (index < stringToSplit.Length)
            {
                var indexOf = stringToSplit.IndexOf(separator, index);
                if (indexOf != -1)
                {
                    offsets[offset++] = indexOf;
                    index = (indexOf + separator.Length);
                }
                else
                {
                    index = stringToSplit.Length;
                }
            }

            var final = new string[offset + 1];
            if (offset == 0) //changed from 1, to fix when no split found
            {
                final[0] = stringToSplit;
            }
            else
            {
                offset--;

                final[0] = stringToSplit.Substring(0, offsets[0]);
                for (var i = 0; i < offset; i++)
                {
                    final[i + 1] = stringToSplit.Substring(offsets[i] + separator.Length, offsets[i + 1] - offsets[i] - separator.Length);
                }
                final[offset + 1] = stringToSplit.Substring(offsets[offset] + separator.Length);
            }

            return final;
        }
    }
    
    public static class EncodingForBase64
    {
        public static string EncodeBase64(this System.Text.Encoding encoding, string text)
        {
            if (text == null)
            {
                return null;
            }

            byte[] textAsBytes = encoding.GetBytes(text);
            return System.Convert.ToBase64String(textAsBytes);
        }

        public static string DecodeBase64(this System.Text.Encoding encoding, string encodedText)
        {
            if (encodedText == null)
            {
                return null;
            }

            byte[] textAsBytes = System.Convert.FromBase64String(encodedText);
            return encoding.GetString(textAsBytes);
        }
    }
}
