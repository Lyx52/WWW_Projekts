using Microsoft.AspNetCore.Identity;

namespace WebProject.Core.Models;

public class ApplicationUser : IdentityUser
{
    public ICollection<Listing> Listings { get; set; } = new List<Listing>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}