using Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class EmailSenderService : IEmailSenderService
{
    private readonly ILogger<EmailSenderService> _logger;
    
    public EmailSenderService(ILogger<EmailSenderService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string email, string title, string message)
    {
        // TODO: Pievienot e-pasta sūtīšanu
        _logger.LogInformation("Email sent {string} -> {string}", email, message);
        return Task.Delay(100);
    }
}