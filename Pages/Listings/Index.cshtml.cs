using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WebProject.Core.Interfaces;
using WebProject.Core.Models;
using WebProject.Infastructure.Services;

namespace WebProject.Pages.Listings;

public class Index : PageModel
{
    [BindProperty]
    public int ListingId { get; set; }
    
    [BindProperty]
    public Listing? Listing { get; set; }

    private readonly IEntityRepository<Listing> _listingRepository;
    
    public Index(IEntityRepository<Listing> repository)
    {
        _listingRepository = repository;
    }
    
    public async Task<IActionResult> OnGetAsync()
    {
        // TODO: FIX 400/404!
        // Ja nav dots parametrs atgriezam nepareizu pieprasījumu
        if (!RouteData.Values.ContainsKey("listingId"))
            return BadRequest();
        
        // Ja parametrs neatbilst parsībām atgriežam kļūdainu pieprasījumu
        var listingId = RouteData.Values["listingId"]?.ToString();
        if (listingId is null || listingId?.Length != 12)
            return BadRequest();
        
        // Dekodējam parametru un atrodam entītiju
        try
        {
            ListingId = listingId.Decode();
        }
        catch (FormatException)
        {
            return BadRequest();
        }

        Listing = await _listingRepository
            .AsQueryable().Include(l => l.Category).Include(l => l.Images)
            .FirstOrDefaultAsync(l => l.Id == ListingId);
        if (Listing is null)
            return NotFound();  
        
        return Page();
    }
}