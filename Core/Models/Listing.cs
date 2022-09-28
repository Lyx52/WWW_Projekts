using System.Buffers.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using WebProject.Infastructure.Services;

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

    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string DisplayPrice => Price > 0 ? string.Format(new CultureInfo("lv-LV"), "{0:c}", Price) : "Free";
    
    [Required(ErrorMessage = "Kategorija ir nepieciešama!")]
    public ListingCategory Category { get; set; }
    
    [Required(ErrorMessage = "Kategorija ir nepieciešama!")]
    public int CategoryId { get; set; } = 0;

    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string ListingUrlId => DataProtectorService.Encode(BitConverter.GetBytes(Id));
    public List<ListingImage> Images { get; set; } = new List<ListingImage>();
}