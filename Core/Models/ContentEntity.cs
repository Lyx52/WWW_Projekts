using System.ComponentModel.DataAnnotations;

namespace WebProject.Core.Models;

public class ContentEntity : IdEntity
{
    public DateTime? Created { get; set; } = DateTime.UtcNow;

    public DateTime? Modified { get; set; } = DateTime.MaxValue;

    public ApplicationUser? User { get; set; }
}