﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebProject.Core.Interfaces;
using WebProject.Core.Models;

namespace WebProject.Pages.Account;

public class Listings : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEntityRepository<Listing> _listingRepository;
    private readonly IEntityRepository<ListingImage> _listingImageRepository;
    public Listings(UserManager<ApplicationUser> userManager, IEntityRepository<Listing> listingRepository, IEntityRepository<ListingImage> listingImageRepository)
    {
        _userManager = userManager;
        _listingRepository = listingRepository;
        _listingImageRepository = listingImageRepository;
    }
    public List<Listing> UserListings { get; set; } = new List<Listing>();
    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return Unauthorized();

        UserListings = await _listingRepository.AsQueryable()
            .Include(l => l.Category)
            .Include(l => l.CreatedBy)
            .Where(l => l.CreatedByKey == user.Id)
            .ToListAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostRemoveListingAsync(int? listingId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return Unauthorized();
        var listing = await _listingRepository.AsQueryable()
            .Include(l => l.Category)
            .Include(l => l.CreatedBy)
            .Where(l => l.CreatedByKey == user.Id && l.Id == listingId)
            .FirstOrDefaultAsync();
        if (listing is not null)
        {
            if (listing.Images is not null)
                await _listingImageRepository.RemoveRange(listing.Images);
            await _listingRepository.Remove(listing);
        }

        return Redirect("/Account/Listings");
    }
}