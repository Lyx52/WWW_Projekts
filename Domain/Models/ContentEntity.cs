using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models;

public class ContentEntity : IdEntity
{
    public DateTime? Created { get; set; } = DateTime.UtcNow;

    public DateTime? Modified { get; set; } = DateTime.MaxValue;
    public ApplicationUser? CreatedBy { get; set; }
    
    [ForeignKey(nameof(ApplicationUser))]
    public string? CreatedByKey { get; set; }
}