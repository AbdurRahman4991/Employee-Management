using MyFirstApi.Services.Interfaces;

namespace MyFirstApi.Services.Background
{
    public class EmailBackgroundService : BackgroundService
    {
        private readonly EmailQueueService _emailQueue;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<EmailBackgroundService> _logger;

        public EmailBackgroundService(
            EmailQueueService emailQueue,
            IServiceScopeFactory scopeFactory,
            ILogger<EmailBackgroundService> logger)
        {
            _emailQueue = emailQueue;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Email Background Service started."
            );

            await foreach (
                var email in _emailQueue.Reader.ReadAllAsync(
                    stoppingToken))
            {
                try
                {
                    using var scope =
                        _scopeFactory.CreateScope();

                    var emailService =
                        scope.ServiceProvider
                            .GetRequiredService<IEmailService>();

                    await emailService.SendEmailAsync(
                        email.To,
                        email.Subject,
                        email.Body
                    );

                    _logger.LogInformation(
                        "Email sent successfully to {Email}",
                        email.To
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Failed to send email to {Email}",
                        email.To
                    );
                }
            }
        }
    }
}