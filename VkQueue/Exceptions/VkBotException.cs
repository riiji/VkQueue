using System;

namespace VkQueue.Exceptions
{
    internal class VkBotException : Exception
    {
        public VkBotException(string message)
            : base(message)
        {
            
        }
    }
}
