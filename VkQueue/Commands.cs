using VkQueue.VkObjects;

namespace VkQueue
{
    internal class Commands
    {
        public static Commands Instance = new Commands();

        private static readonly VkModule VkModule = new VkModule();

        public static void Push(long conversationId, long userId)
        {
            // get vk user 
            var user = VkModule.GetUser(VkModule.VkApi, userId);

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

                var messageId = VkModule.SendMessageInConversation(VkQueue.Instance.Message, conversationId,
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
                    VkModule.SendMessageInConversation("Очередь уже создана в другой беседе",
                        conversationId, Utilities.Random.Next());
                    return;
                }

                VkQueue.Instance.Message += $"{user.FirstName} {user.LastName}\n";

                VkModule.EditMessageInConversation(VkQueue.Instance.Message, conversationId,
                    VkQueue.Instance.MessageId);
            }
        }

        public static void PushInPM(long userId)
        {
            if (VkQueue.Instance.MessageId == -1)
            {
                VkModule.SendMessageInPM("Невозможно создать очередь в личных сообщениях",
                    userId, Utilities.Random.Next());
            }
            else
            {
                var user = VkModule.GetUser(VkModule.VkApi, userId);

                if (VkQueue.Instance.Contains(user))
                    return;

                VkQueue.Instance.ConversationQueue.Enqueue(user);

                VkQueue.Instance.Message += $"{user.FirstName} {user.LastName}\n";

                VkModule.EditMessageInConversation(VkQueue.Instance.Message,
                    VkQueue.Instance.ConversationId, VkQueue.Instance.MessageId);
            }
        }

        public static void Pop(long conversationId)
        {
            if (VkQueue.Instance.ConversationQueue.Count == 0)
                return;

            if (VkQueue.Instance.ConversationId != -1 && VkQueue.Instance.ConversationId != conversationId)
            {
                VkModule.SendMessageInConversation("Невозможно вызвать pop в другой беседе",
                    conversationId, Utilities.Random.Next());
                return;
            }

            VkQueue.Instance.ConversationQueue.Dequeue();

            if (VkQueue.Instance.ConversationQueue.Count == 0)
            {
                VkModule.DeleteMessageInConversation(conversationId, VkQueue.Instance.MessageId,
                    true);
            }
            else
            {
                VkQueue.Instance.Message = Utilities.RemoveFirstLine(VkQueue.Instance.Message);

                VkModule.EditMessageInConversation(VkQueue.Instance.Message, conversationId,
                    VkQueue.Instance.MessageId);
            }
        }

        public static void PopInPM(long userId)
        {
            VkModule.SendMessageInPM("Нельзя вызвать pop в личных сообщениях", userId,
                Utilities.Random.Next());
        }

        public static void Up(long conversationId, long userId)
        {
            VkModule.UpMessageInConversation( VkQueue.Instance.Message,
                VkQueue.Instance.ConversationId, VkQueue.Instance.MessageId, Utilities.Random.Next());
        }

        public static void Clear(long conversationId)
        {
            if (conversationId != VkQueue.Instance.ConversationId)
            {
                VkModule.SendMessageInConversation(
                    "Невозможно очистить очередь в другой беседе", conversationId, Utilities.Random.Next());
                return;
            }

            VkModule.DeleteMessageInConversation(VkQueue.Instance.ConversationId,
                VkQueue.Instance.MessageId, true);
        }
    }
}
