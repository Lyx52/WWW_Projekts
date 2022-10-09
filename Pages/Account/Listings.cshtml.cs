using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebProject.Core.Interfaces;
using WebProject.Core.Models;

namespace WebProject.Pages.Account;

public class Listings : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEntityRepository<Listing> _listingRepository;
    public Listings(UserManager<ApplicationUser> userManager, IEntityRepository<Listing> listingRepository)
    {
        _userManager = userManager;
        _listingRepository = listingRepository;
    }
    public List<Listing> UserListings { get; set; } = new List<Listing>();
    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return Unauthorized();

        UserListings = await _listingRepository.AsQueryable()
            .Include(l => l.Category)
            .Include(l => l.CreatedBy)
            .Where(l => l.CreatedBy!.Id == user.Id)
            .ToListAsync();
        return Page();
    }
}