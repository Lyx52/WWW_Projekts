﻿using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace WebProject.Pages.Admin;

public class Dashboard : PageModel
{
    private readonly ILogger<Dashboard> _logger;
    private readonly IEntityRepository<ListingCategory> _categoriesRepository;
    private readonly AppDbContext _dbContext;
    public List<ListingCategory> Categories { get; set; } = new List<ListingCategory>();
    
    [BindProperty]
    public CategoryCreateInputModel CategoryCreateInput { get; set; }
    
    [BindProperty]
    public SearchByKeywordInputModel SearchByKeywordInput { get; set; }

    [BindProperty] 
    public string CurrentTab { get; set; } = "nav-category";

    [BindProperty] public List<ApplicationUser> FoundUsers { get; set; } = new List<ApplicationUser>();
    [BindProperty] public List<Listing> FoundListings { get; set; } = new List<Listing>();
    [BindProperty]
    public CategoryRemoveInputModel CategoryRemoveInput { get; set; }
    
    [BindProperty]
    public string SubmissionUserId { get; set; }
    
    [BindProperty]
    public int RemoveListingId { get; set; }

    [BindProperty]
    public CategoryRenameInputModel CategoryRenameInput { get; set; }
    public Dashboard(ILogger<Dashboard> logger, IEntityRepository<ListingCategory> categories, AppDbContext dbContext)
    {
        _logger = logger;
        _categoriesRepository = categories;
        _dbContext = dbContext;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        Categories = await _categoriesRepository.ToList();
        return Page();
    }
    
    // Formu kontrolieris kas atbildīgs par kategoriju dzēšanu
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
    // Formu kontrolieris kas atbildīgs par kategoriju pārsaukšanu
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
    // Formu kontrolieris kas atbildīgs par kategoriju izveidi
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
    // Formu kontrolieris kas atbildīgs par lietotāju un sludinājumu meklēšanu
    public async Task<IActionResult> OnPostSearchByKeywordAsync()
    {
        if (ModelState.GetFieldValidationState("SearchByKeywordInput") == ModelValidationState.Valid)
        {
            switch (SearchByKeywordInput.SearchType)
            {
                case SearchType.Users:
                    FoundUsers = await FindUsers(SearchByKeywordInput.SearchKeyword);
                    break;
                case SearchType.Listings:
                    FoundListings = await FindListings(SearchByKeywordInput.SearchKeyword);
                break;
            }
        }
        
        return Page();
    }
    // Formu kontrolieris kas atbildīgs par lietotāju bloķēšanu
    public async Task<IActionResult> OnPostChangeUserBlockAsync()
    {
        if (ModelState.GetFieldValidationState("SubmissionUserId") == ModelValidationState.Valid)
        {
            FoundUsers = await FindUsers(SearchByKeywordInput.SearchKeyword);
            var blockUser = FoundUsers.FirstOrDefault(user => user.Id == SubmissionUserId);
            if (blockUser is not null)
            {
                blockUser.LockoutEnd = blockUser.LockoutEnd < DateTime.UtcNow
                    ? DateTime.MaxValue.ToUniversalTime()
                    : DateTime.MinValue.ToUniversalTime();
                _dbContext.Update(blockUser);
                await _dbContext.SaveChangesAsync();    
            }
        }

        return Page();
    }
    // Formu kontrolieris kas atbildīgs par sludinājumu dzēšanu
    public async Task<IActionResult> OnPostRemoveListingAsync()
    {
        if (ModelState.GetFieldValidationState("RemoveListingId") == ModelValidationState.Valid)
        {
            FoundListings = await FindListings(SearchByKeywordInput.SearchKeyword);
            var listing = FoundListings.FirstOrDefault(listing => listing.Id == RemoveListingId);
            if (listing is not null)
            {
                _dbContext.Set<Listing>().Remove(listing);
                await _dbContext.SaveChangesAsync();
            }
        }

        return Page();
    }
    // Formu kontrolieris kas atbildīgs par lietotāju atrašanu
    public async Task<List<ApplicationUser>> FindUsers(string keyword)
    {
        return await _dbContext.Set<ApplicationUser>().Where(user =>
            user.UserName.ToLower(CultureInfo.InvariantCulture)
                .Contains(keyword.ToLower(CultureInfo.InvariantCulture)) ||
            user.Email.ToLower(CultureInfo.InvariantCulture)
                .Contains(keyword.ToLower(CultureInfo.InvariantCulture))
        ).ToListAsync();
    }
    public async Task<List<Listing>> FindListings(string keyword)
    {
        return await _dbContext.Set<Listing>().Where(listing =>
            listing.Title.ToLower(CultureInfo.InvariantCulture).Contains(keyword.ToLower(CultureInfo.InvariantCulture)) ||
            listing.CreatedBy!.UserName.ToLower(CultureInfo.InvariantCulture).Contains(keyword.ToLower(CultureInfo.InvariantCulture)) ||
            listing.CreatedBy!.Email.ToLower(CultureInfo.InvariantCulture).Equals(keyword.ToLower(CultureInfo.InvariantCulture))
        )
            .Include(l => l.Category)
            .Include(l => l.CreatedBy)
            .ToListAsync();
    }

    public class SearchByKeywordInputModel
    {
        [Required(ErrorMessage = "Meklēšanas atslēgvārds ir nepieciešams!")]
        public string SearchKeyword { get; set; }
    
        [BindProperty]
        [Required]
        public SearchType SearchType { get; set; }
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
    public enum SearchType
    {
        Users,
        Listings,
        UserListings
    }
}
