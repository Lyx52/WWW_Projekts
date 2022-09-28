using System.ComponentModel.DataAnnotations;

namespace WebProject.Core.Models;

public class ListingImage : ContentEntity
{
    [Required]
    public string FilePath { get; set; } = string.Empty;
    
    public Listing? Listing { get; set; }
    
    public int ListingId { get; set; }
}