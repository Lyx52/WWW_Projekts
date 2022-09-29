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
        await _messageRepository.Add(new Message() { Created = DateTime.UtcNow, Text = message, CreatedBy = sender, Recipient = recipient });
    }

    public async Task<List<Message>> GetReceivedMessages(ApplicationUser user, int offset = 0, int limit = -1)
    {
        return await _messageRepository
            .AsQueryable().Include(m => m.Recipient)
            .Where(m => m.Recipient!.Id == user.Id)
            .Skip(offset).Take(limit > 0 ? limit : Int32.MaxValue).ToListAsync();
    }
    public async Task<List<Message>> GetSentMessages(ApplicationUser user, int offset = 0, int limit = -1)
    {
        return await _messageRepository
            .AsQueryable().Include(m => m.CreatedBy)
            .Where(m => m.CreatedBy!.Id == user.Id)
            .Skip(offset).Take(limit > 0 ? limit : Int32.MaxValue).ToListAsync();
    }
    public async Task<List<Message>> GetAllMessages(ApplicationUser user, int offset = 0, int limit = -1)
    {
        return await _messageRepository
            .AsQueryable()
            .Include(m => m.CreatedBy)
            .Include(m => m.Recipient)
            .Where(m => m.CreatedBy!.Id == user.Id || m.Recipient!.Id == user.Id)
            .Skip(offset).Take(limit > 0 ? limit : Int32.MaxValue).ToListAsync();
    } 
}