namespace WebProject.Core.Interfaces;

public interface IEmailSenderService
{
    Task SendEmailAsync(string email, string title, string message);
}