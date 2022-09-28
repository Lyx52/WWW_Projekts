namespace WebProject.Core.Interfaces.Services;

public interface IEmailSenderService
{
    Task SendEmailAsync(string email, string title, string message);
}