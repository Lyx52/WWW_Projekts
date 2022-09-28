using System.ComponentModel.DataAnnotations;

namespace WebProject.Core.Models;

public class Message : ContentEntity
{
    [Required]
    public string Text { get; set; } = string.Empty;
    
    [Required]
    public ApplicationUser? Recipient { get; set; }
}