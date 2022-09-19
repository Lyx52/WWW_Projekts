using Microsoft.AspNetCore.Identity;

namespace WebProject.Core.Models;

public class ApplicationUser : IdentityUser
{
    public ICollection<Listing> Listings = new List<Listing>();
}