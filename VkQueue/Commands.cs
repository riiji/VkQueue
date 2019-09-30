using VkQueue.VkObjects;

namespace VkQueue
{
    internal class Commands
    {
        public static Commands Instance = new Commands();

        private readonly VkModule _vkModule = new VkModule();

        public void Push(long conversationId, long userId)
        {
            // get vk user 
            var user = _vkModule.GetUser(userId);

            // check user on null
            if (user == null)
                return;

            // check if user contains in VkQueue
            if (VkQueue.Instance.Contains(user))
                return;

            VkQueue.Instance.ConversationQueue.Enqueue(user);

            if (VkQueue.Instance.MessageId == -1)
            {
                VkQueue.Instance.Message = $"Очередь:\n{user.FirstName} {user.LastName}\n";

                var messageId = _vkModule.SendMessageInConversation(VkQueue.Instance.Message, conversationId,
                    Utilities.Random.Next());

                if (messageId != null)
                {
                    VkQueue.Instance.MessageId = (long) messageId;
                    VkQueue.Instance.ConversationId = conversationId;
                }
            }
            else
            {
                if (VkQueue.Instance.ConversationId != conversationId)
                {
                    _vkModule.SendMessageInConversation("Очередь уже создана в другой беседе",
                        conversationId, Utilities.Random.Next());
                    return;
                }

                VkQueue.Instance.Message += $"{user.FirstName} {user.LastName}\n";

                _vkModule.EditMessageInConversation(VkQueue.Instance.Message, conversationId,
                    VkQueue.Instance.MessageId);
            }
        }

        public void PushInPM(long userId)
        {
            if (VkQueue.Instance.MessageId == -1)
            {
                _vkModule.SendMessageInPM("Невозможно создать очередь в личных сообщениях",
                    userId, Utilities.Random.Next());
            }
            else
            {
                var user = _vkModule.GetUser(userId);

                if (VkQueue.Instance.Contains(user))
                    return;

                VkQueue.Instance.ConversationQueue.Enqueue(user);

                VkQueue.Instance.Message += $"{user.FirstName} {user.LastName}\n";

                _vkModule.EditMessageInConversation(VkQueue.Instance.Message,
                    VkQueue.Instance.ConversationId, VkQueue.Instance.MessageId);
            }
        }

        public void Pop(long conversationId)
        {
            if (VkQueue.Instance.ConversationQueue.Count == 0)
                return;

            if (VkQueue.Instance.ConversationId != -1 && VkQueue.Instance.ConversationId != conversationId)
            {
                _vkModule.SendMessageInConversation("Невозможно вызвать pop в другой беседе",
                    conversationId, Utilities.Random.Next());
                return;
            }

            VkQueue.Instance.ConversationQueue.Dequeue();

            if (VkQueue.Instance.ConversationQueue.Count == 0)
            {
                _vkModule.DeleteMessageInConversation(conversationId, VkQueue.Instance.MessageId,
                    true);
            }
            else
            {
                VkQueue.Instance.Message = Utilities.RemoveFirstLine(VkQueue.Instance.Message);

                _vkModule.EditMessageInConversation(VkQueue.Instance.Message, conversationId,
                    VkQueue.Instance.MessageId);
            }
        }

        public void PopInPM(long userId)
        {
            _vkModule.SendMessageInPM("Нельзя вызвать pop в личных сообщениях", userId,
                Utilities.Random.Next());
        }

        public void Up(long conversationId, long userId)
        {
            _vkModule.UpMessageInConversation( VkQueue.Instance.Message,
                VkQueue.Instance.ConversationId, VkQueue.Instance.MessageId, Utilities.Random.Next());
        }

        public void Clear(long conversationId)
        {
            if (conversationId != VkQueue.Instance.ConversationId)
            {
                _vkModule.SendMessageInConversation(
                    "Невозможно очистить очередь в другой беседе", conversationId, Utilities.Random.Next());
                return;
            }

            _vkModule.DeleteMessageInConversation(VkQueue.Instance.ConversationId,
                VkQueue.Instance.MessageId, true);
        }
    }
}
