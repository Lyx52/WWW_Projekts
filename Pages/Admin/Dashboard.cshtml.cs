using Microsoft.AspNetCore.Mvc.RazorPages;
using WebProject.Core.Interfaces;
using WebProject.Core.Models;
using WebProject.Infastructure.Data;

namespace WebProject.Pages.Admin;

public class Dashboard : PageModel
{
    private readonly ILogger<Dashboard> _logger;
    private readonly IEntityRepository<ListingCategory> _categoriesRepository;
    public List<ListingCategory> Categories { get; set; } = new List<ListingCategory>();
    public Dashboard(ILogger<Dashboard> logger, IEntityRepository<ListingCategory> categories)
    {
        _logger = logger;
        _categoriesRepository = categories;
    }
    public string ReturnUrl { get; set; }
    public string Test { get; set; }
    public async void OnGetAsync()
    {
        var parent = await _categoriesRepository.GetById(-1);
        if (parent is not null)
        {
            parent.SubCategories.Add(new ListingCategory() { ParentCategoryId = -1, Name = "Subcategory"});
            await _categoriesRepository.Update(parent);
        }
        Categories = await _categoriesRepository.ToList();
        Test = parent is not null ? parent.SubCategories.Count.ToString() : "None";
    }
}
