using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using VkNet.Infrastructure.Authorization.ImplicitFlow;

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
