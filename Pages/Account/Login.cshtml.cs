using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebProject.Core.Interfaces;
using WebProject.Core.Interfaces.Services;
using WebProject.Core.Models;

namespace WebProject.Pages.Account;

public class Login : PageModel
{
    private readonly ILogger<Login> _logger;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSenderService _emailSenderSender;
    [BindProperty]
    public LoginInputModel Input { get; set; }
    
    public string ReturnUrl { get; set; }

    public Login(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<Login> logger)
    {
        _logger = logger;
        _signInManager = signInManager;
        _userManager = userManager;
    }
    public void OnGet()
    {
        
    }
    
    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl = returnUrl ?? Url.Content("~/Index");
        if (ModelState.IsValid)
        {
            // Mēģinam atrast lietotāju pēc epasta ja atrodam ielogojamies pēc tā lietotājvārda
            var user = await _userManager.FindByEmailAsync(Input.EmailOrUsername);
            string username = user is null ? Input.EmailOrUsername : user.UserName;
            
            var result = await _signInManager.PasswordSignInAsync(username, Input.Password, Input.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            ModelState.AddModelError(string.Empty, "Neizdevās pieslēgties! Nepareizs lietotāja vārds vai parole!");
        }
        
        // Redisplay form
        return Page();
    }
    public class LoginInputModel
    {
        [Required(ErrorMessage = "'E-Pasts vai Lietotājvārds' lauks ir nepieciešams")]
        [Display(Name = "E-Pasts vai Lietotājvārds")]
        public string EmailOrUsername { get; set; }
        
        [Required(ErrorMessage = "'Parole lauks' ir nepieciešams")]
        [DataType(DataType.Password)]
        [Display(Name = "Parole")]
        public string Password { get; set; }
        [Display(Name = "Atcerēties mani")]
        public bool RememberMe { get; set; }
    }
}