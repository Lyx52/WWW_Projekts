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
    public IndexModel(ILogger<IndexModel> logger, IEntityRepository<Listing> listingRepository)
    {
        _logger = logger;
        _listingRepository = listingRepository;
    }

    public async void OnGetAsync(int? listingPage = 0)
    {
        int page = listingPage ?? 0;
        // Izdabūjam pēdējos 24 
        Listings = await _listingRepository.AsQueryable()
            .Include(l => l.Images).Reverse().Skip(page * 24).Take(24).ToListAsync();
    }
}
