using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using VkQueue.VkObjects;

namespace VkQueue
{
    internal static class RequestHelper
    {
        public static async void RequestHandlerAsync(IVkResponse response)
        {
            int value;

            try
            {
                // get type of event
                value = response.Updates[0][0];
            }
            catch
            {
                return;
            }

            // if message recieved
            if (value == 4)
            {
                CommandHandler(response);
            }

            await Task.CompletedTask;

        }

        private static void CommandHandler(IVkResponse response)
        {
            // get a message text
            string text = response.Updates[0][5];

            // get a user id
            VkId userId = Utilities.ConvertJsonToObject<VkId>((response.Updates[0][6] as JObject).ToString());

            // get a conversation id
            long conversationId = response.Updates[0][3];

            text = text.ToLower();

            // get first index of space
            var spaceIndex = text.IndexOf(' ') != -1 ? text.IndexOf(' ') : text.Length;

            // get command from text
            var command = text.Substring(0, spaceIndex);

            // if conversationId < 2000000000, its not a conversation, its a dialog check documentation vk api
            switch (command)
            {
                case "push":

                    if (conversationId < 2000000000)
                        Commands.PushInPM(conversationId);
                    else
                        Commands.Push(conversationId, userId.From);
                    break;
                case "pop":
                    if (conversationId < 2000000000)
                        Commands.PopInPM(conversationId);
                    else
                        Commands.Pop(conversationId);
                    break;
                case "up":
                    if (conversationId >= 2000000000)
                        Commands.Up(conversationId, userId.From);
                    break;
                case "clear":
                    if (conversationId >= 2000000000)
                        Commands.Clear(conversationId);
                    break;
            }
        }
    }
}
