using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebProject.Core.Interfaces;
using WebProject.Core.Models;
namespace WebProject.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    
    public List<Listing> Listings { get; set; } = new List<Listing>();
    private readonly IEntityRepository<Listing> _listingRepository;
    public int PageNumber { get; set; }
    public string? SearchParam { get; set; } = string.Empty;
    public string NextPage => Url.Page("/Index", new { listingPage = PageNumber + 1, search = SearchParam }) ?? "/Index";
    public string PrevPage => Url.Page("/Index", new { listingPage = Math.Max(1, PageNumber - 1), search = SearchParam }) ?? "/Index";
    public IndexModel(ILogger<IndexModel> logger, IEntityRepository<Listing> listingRepository)
    {
        _logger = logger;
        _listingRepository = listingRepository;
    }

    public async Task OnPostSearchListingAsync([FromForm] string? search, [FromRoute] int? listingPage)
    {
        Listings = await GetListings(listingPage, search);
    }
    public async Task OnGetAsync([FromRoute] string? search, [FromRoute] int? listingPage)
    {
        
        // TODO: Fix searching/paging...
        // Izdabūjam pēdējos 24 
        Listings = await GetListings(listingPage, search);
    }

    private async Task<List<Listing>> GetListings(int? listingPage = 1, string? searchParameter=null)
    {
        PageNumber = listingPage ?? 1;
        PageNumber = Math.Max(1, Math.Min(PageNumber, (await _listingRepository.Count() / 24)));
        var query = _listingRepository.AsQueryable()
            .Include(l => l.Images);
        if (!string.IsNullOrEmpty(searchParameter))
        {
            string formattedSearch = searchParameter ?? string.Empty;
            formattedSearch = formattedSearch.ToLowerInvariant();
            SearchParam = searchParameter;
            return await query.Where(l =>
                l.Title.ToLowerInvariant().Contains(formattedSearch) ||
                l.Description.ToLowerInvariant().Contains(formattedSearch))
                .OrderByDescending(l => l.Created)
                .Skip((PageNumber - 1) * 24).Take(24)
                .ToListAsync();
        }

        return await query
            .OrderByDescending(l => l.Created)
            .Skip((PageNumber - 1) * 24).Take(24)
            .ToListAsync();
    }
}
