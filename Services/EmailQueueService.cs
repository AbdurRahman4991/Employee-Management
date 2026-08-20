using System.Threading.Channels;
using MyFirstApi.Services.Interfaces;

namespace MyFirstApi.Services
{
    public class EmailQueueService : IEmailQueueService
    {
        private readonly Channel<EmailQueueItem> _queue;

        public EmailQueueService()
        {
            _queue = Channel.CreateUnbounded<EmailQueueItem>();
        }

        public async ValueTask QueueEmailAsync(
            string to,
            string subject,
            string body)
        {
            var email = new EmailQueueItem
            {
                To = to,
                Subject = subject,
                Body = body
            };

            await _queue.Writer.WriteAsync(email);
        }

        public ChannelReader<EmailQueueItem> Reader =>
            _queue.Reader;
    }

    public class EmailQueueItem
    {
        public string To { get; set; } = "";
        public string Subject { get; set; } = "";
        public string Body { get; set; } = "";
    }
}