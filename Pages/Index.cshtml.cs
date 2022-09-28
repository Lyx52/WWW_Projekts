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
    public int PageNumber = 1;
    public string NextPage => Url.ActionLink("Index", "Home", values: new { listingPage = PageNumber + 1 }) ?? "/Index";
    public string PrevPage => Url.ActionLink("Index", "Home", values: new { listingPage = Math.Max(1, PageNumber - 1) }) ?? "/Index";
    public IndexModel(ILogger<IndexModel> logger, IEntityRepository<Listing> listingRepository)
    {
        _logger = logger;
        _listingRepository = listingRepository;
    }

    public async void OnGetAsync(int? listingPage)
    {
        PageNumber = listingPage ?? 1;
        PageNumber = Math.Max(1, Math.Min(PageNumber, (await _listingRepository.Count() / 24)));
        
        // Izdabūjam pēdējos 24 
        Listings = await _listingRepository.AsQueryable()
            .Include(l => l.Images).OrderByDescending(l => l.Created).Skip((PageNumber - 1) * 24).Take(24).ToListAsync();
    }
}
