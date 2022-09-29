using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebProject.Core.Interfaces.Services;
using WebProject.Core.Models;
using WebProject.Pages.Shared;

namespace WebProject.Controllers;

public class MessageController : Controller
{
    private readonly IDataProtector _dataProtector;
    private readonly IMessageSenderService _messageSender;
    private readonly UserManager<ApplicationUser> _userManager;
    public MessageController(UserManager<ApplicationUser> userManager, IDataProtectionProvider dataProtectionProvider, IMessageSenderService messageSender)
    {
        _dataProtector = dataProtectionProvider.CreateProtector("MessageProvider");
        _messageSender = messageSender;
        _userManager = userManager;
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> MarkAsRead(string? pdata)
    {
        if (pdata is null || pdata.Length <= 0)
            return BadRequest();
        var user = await _userManager.GetUserAsync(User);
        if (user is not null && Int32.TryParse(_dataProtector.Unprotect(pdata), out int id))
        {
            await _messageSender.MarkAsRead(user, id);
        }
        else
        {
            return BadRequest();
        }

        return Ok();
    }
}