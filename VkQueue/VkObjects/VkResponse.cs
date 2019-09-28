namespace VkQueue.VkObjects
{
    internal class VkResponse : IVkResponse
    {
        public int Ts { get; set; }

        public dynamic[] Updates { get; set; }
    }
}
