using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebProject.Core.Models;

public class ContentEntity : IdEntity
{
    public DateTime? Created { get; set; } = DateTime.UtcNow;

    public DateTime? Modified { get; set; } = DateTime.MaxValue;
    public ApplicationUser? CreatedBy { get; set; }
    
    [ForeignKey(nameof(ApplicationUser))]
    public string? CreatedByKey { get; set; }
}