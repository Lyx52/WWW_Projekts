using System.ComponentModel.DataAnnotations;

namespace WebProject.Core.Models;

public class Listing : ContentEntity
{
    [Required(ErrorMessage = "Virsraksts ir nepieciešams")]
    [MinLength(8, ErrorMessage = "Virsraksts nav pietiekami garš")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Apraksts ir nepieciešams")]
    [MinLength(20, ErrorMessage = "Apraksts nav pietiekami garš")]
    public string Description { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Cena ir nepieciešama")]
    [DisplayFormat(DataFormatString = "2dp")]
    public float Price { get; set; }
    
    [Required(ErrorMessage = "Kategorija ir nepieciešama!")]
    public ListingCategory Category { get; set; }
    
    [Required(ErrorMessage = "Kategorija ir nepieciešama!")]
    public int CategoryId { get; set; } = 0;
    public List<ListingImage> Images { get; set; } = new List<ListingImage>();
}