using System;
using Microsoft.Extensions.DependencyInjection;
using VkNet;
using VkNet.AudioBypassService.Extensions;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.RequestParams;
using VkQueue.Exceptions;

namespace VkQueue.VkObjects
{
    internal class VkModule
    {

        public VkApi VkApi { get; set; }

        public VkQueue VkQueue { get; set; }

        public VkModule(Config cfg)
        {
            GetVkApi(cfg);
            VkQueue = new VkQueue();
        }
        

        public VkApi GetVkApi(Config cfg)
        {
            var services = new ServiceCollection();
            services.AddAudioBypass();
            VkApi = new VkApi(services);

            try
            {
                VkApi.Authorize(new ApiAuthParams
                {
                    Login = cfg.Login,
                    Password = cfg.Password,
                    Settings = Settings.All,
                });

                Log.Message("VkModule", "Auth successful");
            }
            catch(Exception e)
            {
                throw new VkBotException($"Can't get vk api. Error : {e.Message}");
            }

            return VkApi;

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

                Log.Message("VkModule", $"Message {messageId} sended");

                return messageId;
            }
            catch(Exception e)
            {
                throw new VkBotException($"Can't send message in conversation. Error : {e.Message}");
            }
        }

        public void EditMessageInConversation(string message, long conversationId, long messageId)
        {
            try
            {
                VkApi.Messages.Edit(new MessageEditParams
                {
                    MessageId = VkQueue.MessageId,
                    PeerId = conversationId,
                    Message = VkQueue.Message
                });

                VkQueue.Message = message;

                Log.Message("VkModule", $"Message {messageId} edited");

            }
            catch(Exception e)
            {
                throw new VkBotException($"Can't edit message in conversation. Error: {e.Message}");
            }
        }

        public void DeleteMessageInConversation(long conversationId, long messageId, bool clear)
        {
            try
            {
                VkApi.Messages.Delete(new[] { (ulong)messageId }, null, null, true);

                if (clear)
                    VkQueue = new VkQueue();

                Log.Message("VkModule", $"Message {messageId} deleted");
            }
            catch(Exception e)
            {
                throw new VkBotException($"Can't delete message in conversation. Error : {e.Message}");
            }
        }

        public void UpMessageInConversation(string message, long conversationId, long messageId, int randomId)
        {
            DeleteMessageInConversation(conversationId, messageId,false);

            var newMessageId = SendMessageInConversation(message, conversationId, randomId);

            if (newMessageId != null)
                VkQueue.MessageId = (long)newMessageId;
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

                Log.Message("VkModule", $"Message {messageId} deleted");

                return messageId;

            }
            catch(Exception e)
            {
                throw new VkBotException($"Can't send message in PM. Error : {e.Message}");
            }
        }

        public User GetUser(long userId)
        {
            try
            {
                return VkApi.Users.Get(new[] { userId })[0];
            }
            catch(Exception e)
            {
                throw new VkBotException($"Can't get user. Error : {e.Message}");
            }
        }


    }
}
