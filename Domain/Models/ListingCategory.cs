using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public class ListingCategory : IdEntity
{
    [Required]
    [MinLength(8, ErrorMessage = "Kategorijas nosaukums nav pietiekami garš!")]
    public string Name { get; set; }
    public int? ParentCategoryId { get; set; }
    public ListingCategory? ParentCategory { get; set; }
    public ICollection<ListingCategory>? SubCategories { get; set; } = new List<ListingCategory>();
}