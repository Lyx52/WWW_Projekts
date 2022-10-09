using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebProject.Core.Interfaces;
using WebProject.Core.Models;
using WebProject.Infastructure.Data;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace WebProject.Pages.Admin;

public class Dashboard : PageModel
{
    private readonly ILogger<Dashboard> _logger;
    private readonly IEntityRepository<ListingCategory> _categoriesRepository;
    public List<ListingCategory> Categories { get; set; } = new List<ListingCategory>();
    
    [BindProperty]
    public CategoryCreateInputModel CategoryCreateInput { get; set; }
    
    [BindProperty]
    public CategoryRemoveInputModel CategoryRemoveInput { get; set; }

    [BindProperty]
    public CategoryRenameInputModel CategoryRenameInput { get; set; }
    public Dashboard(ILogger<Dashboard> logger, IEntityRepository<ListingCategory> categories)
    {
        _logger = logger;
        _categoriesRepository = categories;
    }
    
    public async Task<IActionResult> OnPostRemoveCategoryAsync()
    {
        if (ModelState.GetFieldValidationState("CategoryRemoveInput") == ModelValidationState.Valid)
        {
            if (CategoryRemoveInput.CategoryId!.Value == -1)
            {
                ModelState.AddModelError("CategoryRemoveInput.CategoryId", "Nekategorizēto kategoriju nevar izdzēst!");
                Categories = await _categoriesRepository.ToList();
                return Page();    
            }
            var category = await _categoriesRepository.AsQueryable()
                .Include(c => c.SubCategories)
                .FirstOrDefaultAsync(c => c.Id == CategoryRemoveInput.CategoryId!.Value);
            if (category is null)
            {
                ModelState.AddModelError("CategoryRemoveInput.CategoryId", "Kategorija neēksistē!");
                Categories = await _categoriesRepository.ToList();
                return Page();
            }
            await _categoriesRepository.Remove(category);
        }
        Categories = await _categoriesRepository.ToList();
        return Page();
    }
    public async Task<IActionResult> OnPostRenameCategoryAsync()
    {
        if (ModelState.GetFieldValidationState("CategoryRenameInput") == ModelValidationState.Valid)
        {
            if (CategoryRenameInput.CategoryId!.Value == -1)
            {
                ModelState.AddModelError("CategoryRenameInput.CategoryId", "Nekategorizēto kategoriju nevar pārsaukt!");
                Categories = await _categoriesRepository.ToList();
                return Page();    
            }
            var listing = await _categoriesRepository.AsQueryable()
                .FirstOrDefaultAsync(c => c.Id == CategoryRenameInput.CategoryId!.Value);
            if (listing is null)
            {
                ModelState.AddModelError("CategoryRenameInput.CategoryId", "Kategorija neēksistē!");
                Categories = await _categoriesRepository.ToList();
                return Page();
            }

            listing.Name = CategoryRenameInput.Name;
            await _categoriesRepository.Update(listing);
            return LocalRedirect("/Admin/Dashboard");
        }
        Categories = await _categoriesRepository.ToList();
        return Page();
    }
    public async Task<IActionResult> OnPostCreateCategoryAsync()
    {
        if (ModelState.GetFieldValidationState("CategoryCreateInput") == ModelValidationState.Valid)
        {
            var parent = await _categoriesRepository.AsQueryable()
                .Include(c => c.SubCategories)
                .FirstOrDefaultAsync(c => c.Id == CategoryCreateInput.ParentId!.Value);
            if (parent is null)
            {
                ModelState.AddModelError("CategoryCreateInput.ParentId", "Virskategorija neēksistē!");
                Categories = await _categoriesRepository.ToList();
                return Page();
            }

            var newCategory = new ListingCategory
            {
                Name = CategoryCreateInput.Name, ParentCategory = parent, ParentCategoryId = CategoryCreateInput.ParentId!.Value
            };
            await _categoriesRepository.Add(newCategory);
            parent.SubCategories.Add(newCategory);
            await _categoriesRepository.Update(parent);
            return LocalRedirect("/Admin/Dashboard");
        }
        Categories = await _categoriesRepository.ToList();
        return Page();
    }
    public async void OnGetAsync()
    {
        Categories = await _categoriesRepository.ToList();
    }

    public class CategoryRemoveInputModel
    {
        [Required(ErrorMessage = "Kategorija ir nepieciešama!")]
        public int? CategoryId { get; set; } = null!;
    }
    public class CategoryRenameInputModel
    {
        [Required(ErrorMessage = "Kategorija ir nepieciešama!")]
        public int? CategoryId { get; set; } = null!;
        [Required(ErrorMessage = "Kategorijas jaunais nosaukums ir nepieciešams!")]
        public string Name { get; set; }
    }
    public class CategoryCreateInputModel
    {
        [Required(ErrorMessage = "Virskategorija ir nepieciešama!")]
        public int? ParentId { get; set; } = null!;
        [Required(ErrorMessage = "Kategorijas nosaukums ir nepieciešams!")]
        public string Name { get; set; }
    }
}
