using Microsoft.AspNetCore.Identity;

namespace Domain.Models;

public class ApplicationUser : IdentityUser
{
    public ICollection<Listing> Listings { get; set; } = new List<Listing>();
    public ICollection<Message> RecvMessages { get; set; } = new List<Message>();
    public ICollection<Message> SentMessages { get; set; } = new List<Message>();
    public ICollection<ListingImage> Images { get; set; } = new List<ListingImage>();
}