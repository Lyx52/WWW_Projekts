using System.ComponentModel.DataAnnotations;

namespace WebProject.Core.Models;

public class Listing : IdEntity
{
    [Required(ErrorMessage = "Apraksts ir nepieciešams")]
    [MinLength(20, ErrorMessage = "Apraksts nav pietiekami garš")]
    public string Description { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Cena ir nepieciešama")]
    [DisplayFormat(DataFormatString = "2dp")]
    public float Price { get; set; }
}