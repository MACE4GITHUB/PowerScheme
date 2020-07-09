using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerScheme.Utility
{
    public static class Strings
    {
        public static string[] SplitByString(this string stringToSplit, string separator)
        {
            int offset = 0;
            int index = 0;
            int[] offsets = new int[stringToSplit.Length + 1];

            while (index < stringToSplit.Length)
            {
                int indexOf = stringToSplit.IndexOf(separator, index);
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

            string[] final = new string[offset + 1];
            if (offset == 0) //changed from 1, to fix when no split found
            {
                final[0] = stringToSplit;
            }
            else
            {
                offset--;

                final[0] = stringToSplit.Substring(0, offsets[0]);
                for (int i = 0; i < offset; i++)
                {
                    final[i + 1] = stringToSplit.Substring(offsets[i] + separator.Length, offsets[i + 1] - offsets[i] - separator.Length);
                }
                final[offset + 1] = stringToSplit.Substring(offsets[offset] + separator.Length);
            }

            return final;
        }
    }
}
