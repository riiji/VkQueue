using System;
using System.Collections.Generic;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace VkQueue
{
    internal class Commands
    {
        public static void Push(long peerId, long userId)
        {
            var user = VkQueue.VkApi.Users.Get(new[] { userId }, ProfileFields.All)[0];

            if (VkQueue.ObjContains(user))
                return;

            VkQueue.PeerQueue.Enqueue(user);

            try
            {
                if (VkQueue.MessageId == -1)
                {
                    VkQueue.Message = $"Очередь:\n{user.FirstName} {user.LastName}\n";
                    VkQueue.MessageId = VkQueue.VkApi.Messages.Send(new MessagesSendParams
                    {
                        PeerId = peerId,
                        RandomId = Utilities.Random.Next(),
                        Message = VkQueue.Message
                    });
                }
                else
                {
                    VkQueue.Message = VkQueue.Message + $"{user.FirstName} {user.LastName}\n";
                    VkQueue.VkApi.Messages.Edit(new MessageEditParams
                    {
                        MessageId = VkQueue.MessageId,
                        PeerId = peerId,
                        Message = VkQueue.Message
                    });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void Pop(long peerId)
        {
            if (VkQueue.PeerQueue.Count == 0)
                return;

            VkQueue.PeerQueue.Dequeue();

            try
            { 
                if (VkQueue.PeerQueue.Count == 0)
                {
                    VkQueue.VkApi.Messages.Delete(new[] { (ulong)VkQueue.MessageId }, null, null, true);
                    VkQueue.MessageId = -1;
                    VkQueue.Message = "";
                }
                else
                {
                    VkQueue.Message = Utilities.RemoveFirstLine(VkQueue.Message);

                    VkQueue.VkApi.Messages.Edit(new MessageEditParams
                    {
                        MessageId = VkQueue.MessageId,
                        PeerId = peerId,
                        Message = VkQueue.Message,
                    });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void Up(long peerId, long userId)
        {
            try
            {
                VkQueue.VkApi.Messages.Delete(new[] { (ulong)VkQueue.MessageId }, null, null, true);
                VkQueue.MessageId = VkQueue.VkApi.Messages.Send(new MessagesSendParams
                {
                    PeerId = peerId,
                    RandomId = Utilities.Random.Next(),
                    Message = VkQueue.Message,
                });

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void Clear()
        {
            try
            {
                VkQueue.VkApi.Messages.Delete(new[] {(ulong) VkQueue.MessageId}, null, null, true);
                VkQueue.MessageId = -1;
                VkQueue.Message = "";
                VkQueue.PeerQueue = new Queue<User>();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }
}
