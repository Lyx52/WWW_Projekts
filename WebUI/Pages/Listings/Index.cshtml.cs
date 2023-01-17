using Domain.Interfaces;
using Domain.Interfaces.Services;
using Domain.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebProject.Pages.Listings;

public class Index : PageModel
{
    public int ListingId { get; set; }
    
    public Listing? Listing { get; set; }

    public bool MessageSent { get; set; } = false;
    
    [BindProperty]
    public string Message { get; set; }

    private readonly IEntityRepository<Listing> _listingRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMessageSenderService _messageSender;
    public Index(UserManager<ApplicationUser> userManager, IEntityRepository<Listing> listingRepository, IMessageSenderService messageSender)
    {
        _listingRepository = listingRepository;
        _userManager = userManager;
        _messageSender = messageSender;
    }
    public async Task<IActionResult> OnGetAsync(bool? sentSuccess)
    {
        // Ja ziņojums nosūtīts, parādīt paziņojumu lietotājam
        MessageSent = sentSuccess.HasValue && sentSuccess.Value;
        
        // Ja nav dots parametrs atgriežam badrequest
        if (!RouteData.Values.ContainsKey("listingId"))
            return BadRequest();
        
        // Ja parametrs nav pārveidojam atgriežam badrequest
        if (!TryParseUrlId((string?)RouteData.Values["listingId"], out int listingId))
            return BadRequest();
        
        ListingId = listingId;
        Listing = await _listingRepository
            .AsQueryable()
            .Include(l => l.Category)
                .ThenInclude(c => c.ParentCategory)
                .ThenInclude(c => c.ParentCategory)
            .Include(l => l.Images)
            .Include(l => l.CreatedBy)
            .FirstOrDefaultAsync(l => l.Id == ListingId);
        if (Listing is null)
            return NotFound();  
        
        return Page();
    }

    private bool TryParseUrlId(string? value, out int id)
    {
        id = -1;
        // Pārbaudam parametru
        if (value is null || value?.Length != 12)
            return false;
        
        // Dekodējam parametru
        try
        {
            id = value.Decode();
        }
        catch (FormatException)
        {
            return false;
        }

        return true;
    }
}