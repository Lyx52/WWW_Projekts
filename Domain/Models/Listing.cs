using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Domain.Models;

public class Listing : ContentEntity
{
    [Required(ErrorMessage = "Virsraksts ir nepieciešams")]
    [MinLength(8, ErrorMessage = "Virsraksts nav pietiekami garš")]
    [DisplayName("Virsraksts")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Apraksts ir nepieciešams")]
    [MinLength(20, ErrorMessage = "Apraksts nav pietiekami garš")]
    [DisplayName("Apraksts")]
    public string Description { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Cena ir nepieciešama")]
    [DisplayFormat(DataFormatString = "2dp")]
    public float Price { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string DisplayPrice => Price > 0 ? string.Format(new CultureInfo("lv-LV"), "{0:c}", Price) : "Free";
    
    // Todo: Pievienojam kategoriju
    // [Required(ErrorMessage = "Kategorija ir nepieciešama!")]
    public ListingCategory? Category { get; set; }
    
    [ForeignKey(nameof(Category))]
    public int? CategoryId { get; set; }
    
    public List<ListingImage>? Images { get; set; } = new List<ListingImage>();
}