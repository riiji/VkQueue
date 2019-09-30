using System.Runtime.Serialization;

namespace VkQueue
{
    [DataContract]
    public class Config
    {
        [DataMember]
        public string Login { get; set; }
        [DataMember]
        public string Password { get; set; }

    }
}
