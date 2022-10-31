using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebProject.Core.Interfaces;
using WebProject.Core.Interfaces.Services;
using WebProject.Core.Models;

namespace WebProject.Infastructure.Services;

public class MessageSenderService : IMessageSenderService
{
    private readonly IEntityRepository<Message> _messageRepository;
    private readonly ILogger<MessageSenderService> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    public MessageSenderService(UserManager<ApplicationUser> userManager, IEntityRepository<Message> messageRepository, ILogger<MessageSenderService> logger)
    {
        _messageRepository = messageRepository;
        _logger = logger;
        _userManager = userManager;
    }

    public async Task SendMessage(string message, ApplicationUser sender, ApplicationUser recipient)
    {
        await _messageRepository.Add(new Message() { Created = DateTime.UtcNow, Text = message, CreatedByKey = sender.Id, RecipientKey = recipient.Id });
    }

    public async Task<List<Message>> GetReceivedMessages(ApplicationUser? user, int offset = 0, int limit = -1)
    {
        if (user is null)
            return new List<Message>();
        return await _messageRepository
            .AsQueryable().Include(m => m.Recipient)
            .Where(m => m.RecipientKey == user.Id)
            .Skip(offset).Take(limit > 0 ? limit : Int32.MaxValue).ToListAsync();
    }
    public async Task<List<Message>> GetSentMessages(ApplicationUser user, int offset = 0, int limit = -1)
    {
        return await _messageRepository
            .AsQueryable().Include(m => m.CreatedBy)
            .Where(m => m.CreatedByKey == user.Id)
            .Skip(offset).Take(limit > 0 ? limit : Int32.MaxValue).ToListAsync();
    }
    public async Task<List<Message>> GetAllMessages(ApplicationUser? user, int offset = 0, int limit = -1)
    {
        if (user is null)
            return new List<Message>();
        return await _messageRepository
            .AsQueryable()
            .Include(m => m.CreatedBy)
            .Include(m => m.Recipient)
            .Where(m => m.CreatedByKey == user.Id || m.RecipientKey == user.Id)
            .Skip(offset).Take(limit > 0 ? limit : Int32.MaxValue).ToListAsync();
    }

    public async Task<Message?> GetMessageById(int id)
    {
        return await _messageRepository
            .AsQueryable()
            .Include(m => m.CreatedBy)
            .Include(m => m.Recipient)
            .Where(m => m.Id == id)
            .FirstOrDefaultAsync();
    }
    public async Task MarkAsRead(int? messageId)
    {
        var msg = await _messageRepository.AsQueryable()
            .Include(m => m.Recipient)
            .Where(m => m.Id == messageId).FirstOrDefaultAsync();
        if (msg is not null)
        {
            msg.Unread = false;
            await _messageRepository.Update(msg);
        }

    }
}