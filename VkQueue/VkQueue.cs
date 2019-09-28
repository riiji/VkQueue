using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using VkNet;
using VkNet.Model;

namespace VkQueue
{ 
    internal static class VkQueue
    {
        public static Queue<User> PeerQueue { get; set; } = new Queue<User>();
        
        public static VkApi VkApi { get; set; }

        public static long MessageId { get; set; } = -1;

        public static string Message { get; set; } = "";


        public static bool ObjContains(User obj)
        {
            var userEquality = new UserEquality();
            
            return PeerQueue.Contains(obj, userEquality);
        }
    }

    internal class UserEquality : IEqualityComparer<User>
    {
        public bool Equals([AllowNull] User x, [AllowNull] User y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;
            return x.Id == y.Id;
        }

        public int GetHashCode([DisallowNull] User obj)
        {
            return (int)obj.Id;
        }
    }
}
