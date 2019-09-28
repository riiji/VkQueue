namespace VkQueue.VkObjects
{
    interface IVkResponse
    {
        int Ts { get; set; }

        dynamic[] Updates { get; set; }
    }
}
