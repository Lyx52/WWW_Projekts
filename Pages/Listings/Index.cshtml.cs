﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebProject.Core.Interfaces;
using WebProject.Core.Interfaces.Services;
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
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMessageSenderService _messageSender;
    public Index(UserManager<ApplicationUser> userManager, IEntityRepository<Listing> listingRepository, IMessageSenderService messageSender)
    {
        _listingRepository = listingRepository;
        _userManager = userManager;
        _messageSender = messageSender;
    }
    public async Task<IActionResult> OnPostAsync(string? message)
    {
        if (!TryParseUrlId((string?)RouteData.Values["listingId"], out int listingId))
            return BadRequest();
        if (message is null || message.Length <= 0)
            return BadRequest();
        var user = await _userManager.GetUserAsync(User);
        
        if (user is null)
            return BadRequest();
        
        var listing = await _listingRepository
            .AsQueryable()
            .Include(l => l.CreatedBy)
            .FirstOrDefaultAsync(l => l.Id == listingId);
        if (listing is null)
            return BadRequest();
        
        // TODO: REMOVE TEST CODE
        var recv = await _userManager.FindByEmailAsync("user@email.com");
        if (listing.CreatedBy is null)
            listing.CreatedBy = recv;
        await _listingRepository.Update(listing);

        await _messageSender.SendMessage(message, user, listing.CreatedBy);
        return RedirectToAction("Index", "Listings", new { id = RouteData.Values["listingId"], sentSuccess = true });
    }
    public async Task<IActionResult> OnGetAsync(bool? sentSuccess)
    {
        // Ja ziņojums nosūtīts, parādīt paziņojumu lietotājam
        MessageSent = sentSuccess.HasValue && sentSuccess.Value;
        
        // TODO: FIX 400/404!
        // Ja nav dots parametrs atgriežam badrequest
        if (!RouteData.Values.ContainsKey("listingId"))
            return BadRequest();
        
        // Ja parametrs nav pārveidojam atgriežam badrequest
        if (!TryParseUrlId((string?)RouteData.Values["listingId"], out int listingId))
            return BadRequest();
        
        ListingId = listingId;
        Listing = await _listingRepository
            .AsQueryable()
            .Include(l => l.Category)
            .Include(l => l.Images)
            .Include(l => l.CreatedBy)
            .FirstOrDefaultAsync(l => l.Id == ListingId);
        if (Listing is null)
            return NotFound();  
        
        return Page();
    }

    private bool TryParseUrlId(string? value, out int id)
    {
        id = -1;
        // Pārbaudam parametru
        if (value is null || value?.Length != 12)
            return false;
        
        // Dekodējam parametru
        try
        {
            id = value.Decode();
        }
        catch (FormatException)
        {
            return false;
        }

        return true;
    }
}