using System.ComponentModel.DataAnnotations;

namespace WebProject.Core.Models;

public class ListingCategory : IdEntity
{
    [Required]
    public string Name { get; set; }
    public int ParentCategoryId { get; set; } = 0;
    public ListingCategory? ParentCategory { get; set; }
    public ICollection<ListingCategory> SubCategories { get; set; } = new List<ListingCategory>();
}