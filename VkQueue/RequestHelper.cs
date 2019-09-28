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

            // get a peer id
            long peerId = response.Updates[0][3];

            text = text.ToLower();

            // get first index of space
            var spaceIndex = text.IndexOf(' ') != -1 ? text.IndexOf(' ') : text.Length;

            // get command from text
            var command = text.Substring(0, spaceIndex);

            switch (command)
            {
                case "push":
                    Commands.Push(peerId, userId.From);
                    break;
                case "pop":
                    Commands.Pop(peerId);
                    break;
                case "up":
                    Commands.Up(peerId,userId.From);
                    break;
                case "clear":
                    Commands.Clear();
                    break;
            }
        }
    }
}
