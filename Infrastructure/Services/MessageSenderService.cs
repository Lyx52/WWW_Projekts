using Domain.Interfaces;
using Domain.Interfaces.Services;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

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
        // Nosūtām ziņu
        await _messageRepository.Add(new Message() { Created = DateTime.UtcNow, Text = message, CreatedByKey = sender.Id, RecipientKey = recipient.Id });
    }

    public async Task<List<Message>> GetReceivedMessages(ApplicationUser? user, int offset = 0, int limit = -1)
    {
        // Ja lietotājs eksistē pievienot tam saņemtu ziņu
        if (user is null)
            return new List<Message>();
        return await _messageRepository
            .AsQueryable().Include(m => m.Recipient)
            .Where(m => m.RecipientKey == user.Id)
            .Skip(offset).Take(limit > 0 ? limit : Int32.MaxValue).ToListAsync();
    }
    public async Task<List<Message>> GetSentMessages(ApplicationUser user, int offset = 0, int limit = -1)
    {
        // Iegūt nosūtītās ziņas
        return await _messageRepository
            .AsQueryable().Include(m => m.CreatedBy)
            .Where(m => m.CreatedByKey == user.Id)
            .Skip(offset).Take(limit > 0 ? limit : Int32.MaxValue).ToListAsync();
    }
    public async Task<List<Message>> GetAllMessages(ApplicationUser? user, int offset = 0, int limit = -1)
    {
        // Iegūt visas nosūtītās un saņemtās ziņas
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
        // Iegūt ziņu pēc identifikatora
        return await _messageRepository
            .AsQueryable()
            .Include(m => m.CreatedBy)
            .Include(m => m.Recipient)
            .Where(m => m.Id == id)
            .FirstOrDefaultAsync();
    }
    public async Task MarkAsRead(int? messageId)
    {
        // Atzīmēt ziņu kā lasītu
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