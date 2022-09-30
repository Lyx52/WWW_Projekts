using WebProject.Core.Models;

namespace WebProject.Core.Interfaces.Services;

public interface IMessageSenderService
{
    Task<List<Message>> GetSentMessages(ApplicationUser user, int offset = 0, int limit = -1);
    Task<List<Message>> GetReceivedMessages(ApplicationUser user, int offset = 0, int limit = -1);
    Task<List<Message>> GetAllMessages(ApplicationUser user, int offset = 0, int limit = -1);
    Task SendMessage(string message, ApplicationUser sender, ApplicationUser recipient);
    Task MarkAsRead(int? messageId);
    Task<Message?> GetMessageById(int id);
}