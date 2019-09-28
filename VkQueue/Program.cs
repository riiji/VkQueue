using System.Threading.Tasks;
using Flurl.Http;
using VkNet.Model;
using VkQueue.VkObjects;

namespace VkQueue
{
    internal class Program
    { 
        public async Task MainAsync()
        {
            VkQueue.VkApi = VkAuth.GetVkApi("login", "password");

            var longPollServer = VkQueue.VkApi.Messages.GetLongPollServer();

            LongPollRequestAsync(longPollServer);

            await Task.Delay(-1);
        }

        private async void LongPollRequestAsync(LongPollServerResponse longPollServer)
        {
            var response = await $"https://{longPollServer.Server}?act=a_check&key={longPollServer.Key}&ts={longPollServer.Ts}&wait=25&mode=2&version=2".GetStringAsync();

            var vkResponse = Utilities.ConvertJsonToObject<VkResponse>(response);

            longPollServer.Ts = vkResponse.Ts.ToString();

            await Task.Run(() => LongPollRequestAsync(longPollServer));

            await Task.Run(() => RequestHelper.RequestHandlerAsync(vkResponse));
        }

        private static void Main() => new Program().MainAsync().GetAwaiter().GetResult();
    }
}
