using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WebProject.Core.Interfaces;
using WebProject.Core.Interfaces.Services;
using WebProject.Core.Models;

namespace WebProject.Pages.Account;

public class Register : PageModel
{
    private readonly ILogger<Register> _logger;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IEmailSenderService _emailSenderSender;
    [BindProperty]
    public RegistrationInputModel Input { get; set; }
    public string ReturnUrl { get; set; }

    public Register(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<Register> logger, IEmailSenderService emailSenderService)
    {
        _logger = logger;
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
        _emailSenderSender = emailSenderService;
    }
    public void OnGet()
    {
        
    }
    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl = returnUrl ?? Url.Content("~/Index");
        // Validējam vai ievade ir pareiza
        if (ModelState.IsValid)
        {
            // Izveidojam jaunu lietotāju
            var newUser = new ApplicationUser
            {
                UserName = Input.Username,
                Email = Input.Email
            };

            var result = await _userManager.CreateAsync(newUser, Input.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("User {string} created a new account with password.", newUser.Email);
                
                // Izveidojam e-pasta verifikācijas kodu
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                var confirmationUrl = Url.Action(
                    "ConfirmEmail", "Account",
                    values: new { userId = newUser.Id, code = code },
                    protocol: Request.Scheme);
                
                // Aizsūtam e-pastu
                // TODO: Make a proper email structure.
                await _emailSenderSender.SendEmailAsync(Input.Email, "Apstiprini savu e-pastu",
                    $"Lai apstiprinātu e-pastu spied uz \"Apstiprināt\" <a href='{HtmlEncoder.Default.Encode(confirmationUrl)}'>Apstiprināt</a>.");
                
                // Ieloggojamies kontā
                await _userManager.AddToRoleAsync(newUser, "User");
                await _signInManager.SignInAsync(newUser, isPersistent: false);
                return LocalRedirect(returnUrl);
            }
            // Pievienojam validācijas kļūdas
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        // Redisplay form
        return Page();
    }
    public class RegistrationInputModel
    {
        [Required(ErrorMessage = "'E-Pasts' lauks ir nepieciešams")]
        [EmailAddress(ErrorMessage = "Ievadīts nederīgs e-pasts")]
        [Display(Name = "E-Pasts")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "'Parole lauks' ir nepieciešams")]
        [StringLength(100, ErrorMessage = "Parolei jāsastāv no {2} līdz {1} simboliem", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Parole")]
        public string Password { get; set; }

        [Required(ErrorMessage = "'Parole atkārtoti' lauks ir nepieciešams")]
        [DataType(DataType.Password)]
        [Display(Name = "Parole atkārtoti")]
        [Compare("Password", ErrorMessage = "Paroles nesakrīt")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "'Lietotājvārds' lauks ir nepieciešams")]
        [DataType(DataType.Text)]
        [Display(Name = "Lietotājvārds")]
        public string Username { get; set; }
    }
}