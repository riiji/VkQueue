using System;
using VkNet;
using VkQueue.VkObjects;

namespace VkQueue
{
    internal class Commands
    {
        public static void Push(long conversationId, long userId)
        {
            // get vk user 
            var user = VkModule.GetUser(VkQueue.VkApi, userId);

            // check user on null
            if (user == null)
                return;

            // check if user contains in VkQueue
            if (VkQueue.ObjContains(user))
                return;

            VkQueue.ConversationQueue.Enqueue(user);

            if (VkQueue.MessageId == -1)
            {
                VkQueue.Message = $"Очередь:\n{user.FirstName} {user.LastName}\n";

                var messageId = VkModule.SendMessageInConversation(VkQueue.VkApi, VkQueue.Message, conversationId, Utilities.Random.Next());

                if (messageId != null)
                {
                    VkQueue.MessageId = (long)messageId;
                    VkQueue.ConversationId = conversationId;
                }
            }
            else
            {
                if (VkQueue.ConversationId != conversationId)
                {
                    VkModule.SendMessageInConversation(VkQueue.VkApi, "Очередь уже создана в другой беседе", conversationId, Utilities.Random.Next());
                    return;
                }

                VkQueue.Message += $"{user.FirstName} {user.LastName}\n";

                VkModule.EditMessageInConversation(VkQueue.VkApi, VkQueue.Message, conversationId, VkQueue.MessageId);
            }
        }

        public static void PushInPM(long userId)
        {
            if (VkQueue.MessageId == -1)
            {
                VkModule.SendMessageInPM(VkQueue.VkApi, "Невозможно создать очередь в личных сообщениях", userId, Utilities.Random.Next());
            }
            else
            {
                var user = VkModule.GetUser(VkQueue.VkApi, userId);

                if (VkQueue.ObjContains(user))
                    return;

                VkQueue.ConversationQueue.Enqueue(user);

                VkQueue.Message += $"{user.FirstName} {user.LastName}\n";

                VkModule.EditMessageInConversation(VkQueue.VkApi, VkQueue.Message, VkQueue.ConversationId, VkQueue.MessageId);
            }
        }

        public static void Pop(long conversationId)
        {
            if (VkQueue.ConversationQueue.Count == 0)
                return;

            if (VkQueue.ConversationId != -1 && VkQueue.ConversationId != conversationId)
            {
                VkModule.SendMessageInConversation(VkQueue.VkApi, "Невозможно вызвать pop в другой беседе", conversationId, Utilities.Random.Next());
                return;
            }

            VkQueue.ConversationQueue.Dequeue();

            if (VkQueue.ConversationQueue.Count == 0)
            {
                VkModule.DeleteMessageInConversation(VkQueue.VkApi, conversationId, VkQueue.MessageId, true);
            }
            else
            {
                VkQueue.Message = Utilities.RemoveFirstLine(VkQueue.Message);

                VkModule.EditMessageInConversation(VkQueue.VkApi, VkQueue.Message, conversationId, VkQueue.MessageId);
            }
        }

        public static void PopInPM(long userId)
        {
            VkModule.SendMessageInPM(VkQueue.VkApi, "Нельзя вызвать pop в личных сообщениях", userId, Utilities.Random.Next());
        }

        public static void Up(long conversationId, long userId)
        {
            VkModule.UpMessageInConversation(VkQueue.VkApi,VkQueue.Message,VkQueue.ConversationId,VkQueue.MessageId,Utilities.Random.Next());
        }

        public static void Clear(long conversationId)
        {
            if (conversationId != VkQueue.ConversationId)
            {
                VkModule.SendMessageInConversation(VkQueue.VkApi, "Невозможно очистить очередь в другой беседе", conversationId, Utilities.Random.Next());
                return;
            }

            VkModule.DeleteMessageInConversation(VkQueue.VkApi, VkQueue.ConversationId, VkQueue.MessageId, true);
        }

    }
}
