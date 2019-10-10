using System;
using System.IO;
using System.Threading.Tasks;
using Flurl.Http;
using Newtonsoft.Json;
using VkNet.Model;
using VkQueue.VkObjects;

namespace VkQueue
{
    internal class Program
    {
        private VkModule _vkModule;

        private RequestHelper _requestHelper;

        public void Init()
        {
            var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));

            _vkModule = new VkModule(config);

            _requestHelper = new RequestHelper(new Commands(_vkModule));
        }

        public async Task MainAsync()
        {
            Init();

            var longPollServer = _vkModule.VkApi.Messages.GetLongPollServer();

            LongPollRequestAsync(longPollServer);

            await Task.Delay(-1);
        }

        private async void LongPollRequestAsync(LongPollServerResponse longPollServer)
        {
            try
            {
                var response =
                    await
                        $"https://{longPollServer.Server}?act=a_check&key={longPollServer.Key}&ts={longPollServer.Ts}&wait=25&mode=2&version=2"
                            .GetStringAsync();


                var vkResponse = JsonConvert.DeserializeObject<VkResponse>(response);

                longPollServer.Ts = vkResponse.Ts.ToString();

                await Task.Run(() => LongPollRequestAsync(longPollServer));

                await Task.Run(() => _requestHelper.RequestHandlerAsync(vkResponse));
            }
            catch
            {
                LongPollRequestAsync(_vkModule.VkApi.Messages.GetLongPollServer());
            }
        }

        private static void Main() => new Program().MainAsync().GetAwaiter().GetResult();
    }
}
