namespace MyFirstApi.Services.Interfaces
{
    public interface IEmailQueueService
    {
        ValueTask QueueEmailAsync(
            string to,
            string subject,
            string body);
    }
}