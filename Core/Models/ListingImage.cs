using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebProject.Core.Models;

public class ListingImage : ContentEntity
{
    [Required]
    public string FilePath { get; set; } = string.Empty;

    public bool IsUsed { get; set; } = false;
    public Listing? Listing { get; set; }
    
    [ForeignKey(nameof(ListingImage))]
    public int? ListingId { get; set; }
}