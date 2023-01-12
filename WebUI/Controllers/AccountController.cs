using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace WebProject.Controllers;

[ApiController]
[Route("Account")]
public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AccountController> _logger;
    public AccountController(ILogger<AccountController> logger, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("ConfirmEmail")]
    public async Task<IActionResult> ConfirmEmail(string userId, string code)
    {
        // Pārbaudam vai pareizi parametri atsūtīti un vai lietotājs vispār eksistē
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
        {
            return StatusCode(400);
        }
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null || user.EmailConfirmed)
        {
            return StatusCode(400);
        }
        var result = await _userManager.ConfirmEmailAsync(user, code);
        if (result.Succeeded)
            return RedirectToPage("/Index");
        return StatusCode(400);
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("SignOut")]
    public async Task<IActionResult> SignOut()
    {
        // Ja esam ielogojušies, izloggojamies
        if (_signInManager.IsSignedIn(User))
        {
            await _signInManager.SignOutAsync();    
        }
        return RedirectToPage("/Index");
    }
}