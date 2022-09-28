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
    public int ListingId { get; set; }
    
    public Listing? Listing { get; set; }

    public bool MessageSent { get; set; } = false;
    
    [BindProperty]
    public string Message { get; set; }

    private readonly IEntityRepository<Listing> _listingRepository;
    
    public Index(IEntityRepository<Listing> repository)
    {
        _listingRepository = repository;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        return RedirectToAction("Index", "Listings", new { id = RouteData.Values["listingId"], sentSuccess = true });
    }
    public async Task<IActionResult> OnGetAsync(bool? sentSuccess)
    {
        // Ja ziņojums nosūtīts, parādīt paziņojumu lietotājam
        MessageSent = sentSuccess.HasValue && sentSuccess.Value;
        
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
            .AsQueryable()
            .Include(l => l.Category)
            .Include(l => l.Images)
            .Include(l => l.User)
            .FirstOrDefaultAsync(l => l.Id == ListingId);
        if (Listing is null)
            return NotFound();  
        
        return Page();
    }
}