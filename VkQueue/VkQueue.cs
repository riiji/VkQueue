using System.Collections.Generic;
using System.Linq;
using VkNet.Model;

namespace VkQueue
{ 
    internal class VkQueue
    {
        public Queue<User> ConversationQueue { get; set; } = new Queue<User>();
        
        public long MessageId { get; set; } = -1;

        public long ConversationId { get; set; } = -1;

        public string Message { get; set; } = "";

        public bool Contains(User obj)
        {
            return ConversationQueue.Any(x=>x.Id==obj.Id);
        }
    }
}
