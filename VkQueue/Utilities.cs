using System;
using System.IO;
using Newtonsoft.Json;

namespace VkQueue
{
    internal static class Utilities
    {
        public static Config GetConfig()
        {
            return JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));
        }

        public static Random Random { get; set; } = new Random();

        public static T ConvertJsonToObject<T>(string s)
        {
            return JsonConvert.DeserializeObject<T>(s);
        }

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
