using System.ComponentModel.DataAnnotations;

namespace WebProject.Core.Models;

public class Message : ContentEntity
{
    [Required]
    public string Text { get; set; } = string.Empty;
    
    public ApplicationUser? Recipient { get; set; }
    
    public string? RecipientKey { get; set; }
    public bool Unread { get; set; } = true;
}