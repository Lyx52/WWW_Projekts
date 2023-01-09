using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models;

public class ListingImage : ContentEntity
{
    [Required]
    public string FilePath { get; set; } = string.Empty;
    [Required]
    public string OriginalName { get; set; } = string.Empty;
    public bool IsUsed { get; set; } = false;
    public Listing? Listing { get; set; }
    
    [ForeignKey(nameof(ListingImage))]
    public int? ListingId { get; set; }
}