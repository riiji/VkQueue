using System;

namespace VkQueue
{
    class LogMessage
    {
        private string Message { get; }

        public LogMessage(string source, string message)
        {
            Message = $"{DateTime.Now} | {source} : {message}";
        }

        public override string ToString()
        {
            return Message;
        }
    }
}
