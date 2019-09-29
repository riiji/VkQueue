using System;
using Microsoft.Extensions.DependencyInjection;
using VkNet;
using VkNet.AudioBypassService.Extensions;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace VkQueue.VkObjects
{
    internal class VkModule
    {
        public static VkApi VkApi { get; set; }

        public VkApi GetVkApi(Config cfg)
        {
            var services = new ServiceCollection();
            services.AddAudioBypass();
            var result = new VkApi(services);

            try
            {
                result.Authorize(new ApiAuthParams
                {
                    Login = cfg.Login,
                    Password = cfg.Password,
                    Settings = Settings.All,
                });

                Console.WriteLine(new LogMessage("VkModule", "Auth successful"));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Can't auth cause {e.Message}");
                return null;
            }

            VkApi = result;

            return result;

        }

        public long? SendMessageInConversation(string message, long conversationId, int randomId)
        {
            try
            {
                var messageId = VkApi.Messages.Send(new MessagesSendParams
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

        public void EditMessageInConversation(string message, long conversationId, long messageId)
        {
            try
            {
                VkApi.Messages.Edit(new MessageEditParams
                {
                    MessageId = VkQueue.Instance.MessageId,
                    PeerId = conversationId,
                    Message = VkQueue.Instance.Message
                });

                VkQueue.Instance.Message = message;

                Console.WriteLine(new LogMessage("VkModule", $"Message {messageId} edited"));

            }
            catch (Exception e)
            {
                Console.WriteLine(new LogMessage("VkModule", $"Can't edit message cause {e.Message}'"));
            }
        }

        public void DeleteMessageInConversation(long conversationId, long messageId, bool clear)
        {
            try
            {
                VkApi.Messages.Delete(new[] { (ulong)messageId }, null, null, true);

                if (clear)
                    VkQueue.Instance = new VkQueue();

                Console.WriteLine(new LogMessage("VkModule", $"Message {messageId} deleted"));
            }
            catch (Exception e)
            {
                Console.WriteLine(new LogMessage("VkModule", $"Can't delete message cause {e.Message}'"));
            }
        }

        public void UpMessageInConversation(string message, long conversationId, long messageId, int randomId)
        {
            DeleteMessageInConversation(conversationId, messageId,false);

            var newMessageId = SendMessageInConversation(message, conversationId, randomId);

            if (newMessageId != null)
                VkQueue.Instance.MessageId = (long)newMessageId;
        }

        public long? SendMessageInPM(string message, long userId, int randomId)
        {
            try
            {
                var messageId = VkApi.Messages.Send(new MessagesSendParams
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

        public User GetUser(VkApi api, long userId)
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
