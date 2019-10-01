using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VkQueue.VkObjects;

namespace VkQueue
{
    internal class RequestHelper
    {
        private readonly Commands _commands;

        public RequestHelper(Commands commands)
        {
            _commands = commands;
        }

        public async void RequestHandlerAsync(IVkResponse response)
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

        private void CommandHandler(IVkResponse response)
        {
            // get a message text
            string text = response.Updates[0][5];

            // get a user id
            VkId userId = JsonConvert.DeserializeObject<VkId>((response.Updates[0][6] as JObject).ToString());

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
                case "push" when conversationId >= 2000000000:
                    _commands.Push(conversationId, userId.From);
                    break;

                case "push" when conversationId < 2000000000:
                    _commands.PushInPM(conversationId);
                    break;

                case "pop" when conversationId >= 2000000000:
                    _commands.Pop(conversationId);
                    break;

                case "pop" when conversationId < 2000000000:
                    _commands.PopInPM(conversationId);
                    break;

                case "up" when conversationId >= 2000000000:
                    _commands.Up(conversationId, userId.From);
                    break;

                case "clear" when conversationId >= 2000000000:
                    _commands.Clear(conversationId);
                    break;
            }
        }
    }
}
