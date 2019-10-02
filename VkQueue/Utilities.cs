using System;

namespace VkQueue
{
    internal static class Utilities
    {
        public static string RemoveFirstLine(string s)
        {
            var fi = -1;
            var li = -1;
            var count = 0;

            for (var i = 0; i < s.Length; ++i)
            {
                if (s[i] == '\n')
                    ++count;
                if (count == 1 && fi == -1)
                    fi = i;
                if (count == 2)
                {
                    li = i;
                    break;
                }
            }

            return s.Remove(fi, li - fi);
        }
    }
}
