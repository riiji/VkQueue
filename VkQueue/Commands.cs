using VkQueue.VkObjects;

namespace VkQueue
{
    internal class Commands
    {
        public Commands(VkModule vkModule)
        {
            _vkModule = vkModule;
        }

        private readonly VkModule _vkModule;

        public void Push(long conversationId, long userId)
        {
            // get vk user 
            var user = _vkModule.GetUser(userId);

            // check user on null
            if (user == null)
                return;

            // check if user contains in VkQueue
            if (_vkModule.VkQueue.Contains(user))
                return;
                    
            _vkModule.VkQueue.ConversationQueue.Enqueue(user);

            if (_vkModule.VkQueue.MessageId == -1)
            {
                _vkModule.VkQueue.Message = $"Очередь:\n{user.FirstName} {user.LastName}\n";

                var messageId = _vkModule.SendMessageInConversation(_vkModule.VkQueue.Message, conversationId,
                    Utilities.Random.Next());

                if (messageId != null)
                {
                    _vkModule.VkQueue.MessageId = (long) messageId;
                    _vkModule.VkQueue.ConversationId = conversationId;
                }
            }
            else
            {
                if (_vkModule.VkQueue.ConversationId != conversationId)
                {
                    _vkModule.SendMessageInConversation("Очередь уже создана в другой беседе",
                        conversationId, Utilities.Random.Next());
                    return;
                }

                _vkModule.VkQueue.Message += $"{user.FirstName} {user.LastName}\n";

                _vkModule.EditMessageInConversation(_vkModule.VkQueue.Message, conversationId,
                    _vkModule.VkQueue.MessageId);
            }
        }

        public void PushInPM(long userId)
        {
            if (_vkModule.VkQueue.MessageId == -1)
            {
                _vkModule.SendMessageInPM("Невозможно создать очередь в личных сообщениях",
                    userId, Utilities.Random.Next());
            }
            else
            {
                var user = _vkModule.GetUser(userId);

                if (_vkModule.VkQueue.Contains(user))
                    return;

                _vkModule.VkQueue.ConversationQueue.Enqueue(user);

                _vkModule.VkQueue.Message += $"{user.FirstName} {user.LastName}\n";

                _vkModule.EditMessageInConversation(_vkModule.VkQueue.Message,
                    _vkModule.VkQueue.ConversationId, _vkModule.VkQueue.MessageId);
            }
        }

        public void Pop(long conversationId)
        {
            if (_vkModule.VkQueue.ConversationQueue.Count == 0)
                return;

            if (_vkModule.VkQueue.ConversationId != -1 && _vkModule.VkQueue.ConversationId != conversationId)
            {
                _vkModule.SendMessageInConversation("Невозможно вызвать pop в другой беседе",
                    conversationId, Utilities.Random.Next());
                return;
            }

            _vkModule.VkQueue.ConversationQueue.Dequeue();

            if (_vkModule.VkQueue.ConversationQueue.Count == 0)
            {
                _vkModule.DeleteMessageInConversation(conversationId, _vkModule.VkQueue.MessageId,
                    true);
            }
            else
            {
                _vkModule.VkQueue.Message = Utilities.RemoveFirstLine(_vkModule.VkQueue.Message);

                _vkModule.EditMessageInConversation(_vkModule.VkQueue.Message, conversationId,
                    _vkModule.VkQueue.MessageId);
            }
        }

        public void PopInPM(long userId)
        {
            _vkModule.SendMessageInPM("Нельзя вызвать pop в личных сообщениях", userId,
                Utilities.Random.Next());
        }

        public void Up(long conversationId, long userId)
        {
            _vkModule.UpMessageInConversation( _vkModule.VkQueue.Message,
                _vkModule.VkQueue.ConversationId, _vkModule.VkQueue.MessageId, Utilities.Random.Next());
        }

        public void Clear(long conversationId)
        {
            if (conversationId != _vkModule.VkQueue.ConversationId)
            {
                _vkModule.SendMessageInConversation(
                    "Невозможно очистить очередь в другой беседе", conversationId, Utilities.Random.Next());
                return;
            }

            _vkModule.DeleteMessageInConversation(_vkModule.VkQueue.ConversationId,
                _vkModule.VkQueue.MessageId, true);
        }
    }
}
