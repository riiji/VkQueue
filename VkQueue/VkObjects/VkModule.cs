using System;
using Microsoft.Extensions.DependencyInjection;
using VkNet;
using VkNet.AudioBypassService.Extensions;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace VkQueue.VkObjects
{
    internal static class VkModule
    {
        public static VkApi GetVkApi(string login, string password)
        {
            var services = new ServiceCollection();
            services.AddAudioBypass();
            var result = new VkApi(services);

            try
            {
                result.Authorize(new ApiAuthParams
                {
                    Login = login,
                    Password = password,
                    Settings = Settings.All,
                });

                Console.WriteLine(new LogMessage("VkModule", "Auth successful"));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Can't auth cause {e.Message}");
                return null;
            }

            return result;

        }

        public static long? SendMessageInConversation(VkApi api, string message, long conversationId, int randomId)
        {
            try
            {
                var messageId = api.Messages.Send(new MessagesSendParams
                {
                    PeerId = conversationId,
                    RandomId = randomId,
                    Message = message,
                });

                Console.WriteLine(new LogMessage("VkModule", $"Message {messageId} sended"));

                return messageId;
            }
            catch (Exception e)
            {
                Console.WriteLine(new LogMessage("VkModule", $"Can't send message cause {e.Message}"));
                return null;
            }
        }

        public static void EditMessageInConversation(VkApi api, string message, long conversationId, long messageId)
        {
            try
            {
                api.Messages.Edit(new MessageEditParams
                {
                    MessageId = VkQueue.MessageId,
                    PeerId = conversationId,
                    Message = VkQueue.Message
                });

                VkQueue.Message = message;

                Console.WriteLine(new LogMessage("VkModule", $"Message {messageId} edited"));

            }
            catch (Exception e)
            {
                Console.WriteLine(new LogMessage("VkModule", $"Can't edit message cause {e.Message}'"));
            }
        }

        public static void DeleteMessageInConversation(VkApi api, long conversationId, long messageId, bool clear)
        {
            try
            {
                VkQueue.VkApi.Messages.Delete(new[] { (ulong)messageId }, null, null, true);

                if (clear)
                    Utilities.ClearQueue();

                Console.WriteLine(new LogMessage("VkModule", $"Message {messageId} deleted"));
            }
            catch (Exception e)
            {
                Console.WriteLine(new LogMessage("VkModule", $"Can't delete message cause {e.Message}'"));
            }
        }

        public static void UpMessageInConversation(VkApi api, string message, long conversationId, long messageId, int randomId)
        {
            VkModule.DeleteMessageInConversation(api, conversationId, messageId,false);

            var newMessageId = VkModule.SendMessageInConversation(api, message, conversationId, randomId);

            if (newMessageId != null)
                VkQueue.MessageId = (long)newMessageId;
        }

        public static long? SendMessageInPM(VkApi api, string message, long userId, int randomId)
        {
            try
            {
                var messageId = api.Messages.Send(new MessagesSendParams
                {
                    UserId = userId,
                    RandomId = randomId,
                    Message = message
                });

                Console.WriteLine(new LogMessage("VkModule", $"Message {messageId} deleted"));

                return messageId;

            }
            catch (Exception e)
            {
                Console.WriteLine(new LogMessage("VkModule", $"Can't send message in PM cause {e.Message}"));
                return null;
            }
        }

        public static User GetUser(VkApi api, long userId)
        {
            try
            {
                return api.Users.Get(new[] { userId })[0];
            }
            catch (Exception e)
            {
                Console.WriteLine(new LogMessage("VkModule", $"Can't get user cause {e.Message}"));
                return null;
            }
        }


    }
}
