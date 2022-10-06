using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using WebProject.Core.Interfaces;
using WebProject.Core.Models;

namespace WebProject.Pages.Listings;

public class Create : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEntityRepository<ListingImage> _imageRepository;
    private readonly IEntityRepository<ListingCategory> _categoryRepository;
    private readonly IEntityRepository<Listing> _listingRepository;
    private readonly ILogger<Create> _logger;
    private readonly IHostEnvironment _environment;
    public List<ListingCategory> Categories { get; set; }

    [BindProperty] 
    public CreateListingInputModel CreateListingInput { get; set; } = new CreateListingInputModel();
    
    public Create(IEntityRepository<Listing> listingRepository, IEntityRepository<ListingCategory> categoryRepository, IHostEnvironment environment, ILogger<Create> logger, UserManager<ApplicationUser> userManager, IEntityRepository<ListingImage> imageRepository)
    {
        _userManager = userManager;
        _imageRepository = imageRepository;
        _logger = logger;
        _environment = environment;
        _categoryRepository = categoryRepository;
        _listingRepository = listingRepository;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var root = await _categoryRepository.AsQueryable()
            .Include(c => c.SubCategories)
            .Where(c => c.Id == -1)
            .FirstOrDefaultAsync();
        return Page();
    }
    public class CreateListingInputModel
    {
        [Required(ErrorMessage = "Virsraksts ir nepieciešams")]
        [MinLength(8, ErrorMessage = "Virsraksts nav pietiekami garš")]
        [DisplayName("Virsraksts")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Apraksts ir nepieciešams")]
        [MinLength(20, ErrorMessage = "Apraksts nav pietiekami garš")]
        [DisplayName("Apraksts")]
        public string Description { get; set; } = string.Empty;
        
        public string? Images { get; set; } = string.Empty;
    }
    
    public async Task<List<ListingCategory>> GetCategories()
    {
        return await _categoryRepository.AsQueryable()
            .Include(c => c.SubCategories).Include(c => c.ParentCategory)
            .ThenInclude(c => c.SubCategories).Include(c => c.ParentCategory)
            .ThenInclude(c => c.SubCategories).Include(c => c.ParentCategory)
            .ToListAsync();
    }
}