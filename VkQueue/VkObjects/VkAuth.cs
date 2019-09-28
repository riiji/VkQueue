using Microsoft.Extensions.DependencyInjection;
using VkNet;
using VkNet.AudioBypassService.Extensions;
using VkNet.Enums.Filters;
using VkNet.Model;

namespace VkQueue.VkObjects
{
    internal static class VkAuth
    {
        public static VkApi GetVkApi(string login, string password)
        {
            var services = new ServiceCollection();
            services.AddAudioBypass();
            var result = new VkApi(services);

            result.Authorize(new ApiAuthParams
            {
                Login = login,
                Password = password,
                Settings = Settings.All,
            });

            return result;

        }
    }
}
