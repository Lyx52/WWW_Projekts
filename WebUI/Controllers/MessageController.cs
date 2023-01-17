using Domain.Interfaces;
using Domain.Interfaces.Services;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace WebProject.Controllers;

[ApiController]
[Route("Message")]
public class MessageController : Controller
{
    private readonly IDataProtector _dataProtector;
    private readonly IMessageSenderService _messageSender;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<MessageController> _logger;
    private readonly IEntityRepository<Listing> _listingRepository;
    public MessageController(ILogger<MessageController> logger, UserManager<ApplicationUser> userManager, IDataProtectionProvider dataProtectionProvider, IMessageSenderService messageSender, IEntityRepository<Listing> listingRepository)
    {
        _dataProtector = dataProtectionProvider.CreateProtector("MessageProvider");
        _messageSender = messageSender;
        _userManager = userManager;
        _logger = logger;
        _listingRepository = listingRepository;
    }
    
    [HttpPost]
    [Authorize]
    [Route("MarkAsRead")]
    public async Task<IActionResult> MarkAsRead([FromForm]string? pdata)
    {
        if (pdata is null || pdata.Length <= 0)
            return NoContent();
        try
        {
            var user = await _userManager.GetUserAsync(User);
            // messageId|redirectUrl
            // Atkodējam datus
            var data = _dataProtector.Unprotect(pdata).Split("|");
            if (user is not null && Int32.TryParse(data[0], out int id))
            {
                // Ja lietotājs eksistē mēģinam atzīmēt ka ziņa ir lasīta
                await _messageSender.MarkAsRead(id);
            }
            else
            {
                return LocalRedirect(data[1]);
            }
            
            return LocalRedirect(data[1]);
        }
        catch (Exception e)
        {
            _logger.LogError("Unhandled exception: {String}", e.Message);
        }

        return NoContent();
    }

    [HttpPost]
    [Authorize]
    [Route("SendMessage")]
    public async Task<IActionResult> SendMessage([FromForm]string? pdata, [FromForm]string? message)
    {
        if (string.IsNullOrEmpty(pdata) || string.IsNullOrEmpty(message))
            return NoContent();
        try
        {
            var sender = await _userManager.GetUserAsync(User);
            
            // recipientId|returnUrl(optional)
            // Atkodējam datus
            var data = _dataProtector.Unprotect(pdata).Split("|");
            
            if (sender is not null)
            {
                // Atrodam saņēmēju
                var recipient = await _userManager.FindByIdAsync(data[0]);
                
                // Ja eksistē nosūtām ziņu
                if (recipient is not null)
                    await _messageSender.SendMessage(message, sender, recipient);
                
                return data.Length >= 2 ? LocalRedirect(data[1]) : NoContent();
            }
        }
        catch (Exception e)
        {
            _logger.LogError("Unhandled exception: {String}", e.Message);
        }
        return NoContent();
    }
}