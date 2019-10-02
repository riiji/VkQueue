using System;
using System.Runtime.InteropServices;

namespace VkQueue
{
    static class Log
    {
        public static void Message(string source, string message)
        {
            Console.WriteLine($"{DateTime.Now} | {source} : {message}");
        }
    }
}
