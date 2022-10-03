using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WebProject.Core.Models;

namespace WebProject.Pages.Listings;

public class Create : PageModel
{
    [BindProperty]
    public Listing Input { get; set; } = new Listing();

    public async Task<IActionResult> OnPostAsync(string? images)
    {
        if (ModelState.IsValid)
        {
            
        }

        return Page();
    }
    public class CreateListingInputModel
    {
        [Required(ErrorMessage = "Virsraksts ir nepieciešams")]
        [MinLength(8, ErrorMessage = "Virsraksts nav pietiekami garš")]
        [DisplayName("Virsraksts")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Apraksts ir nepieciešams")]
        [MinLength(20, ErrorMessage = "Apraksts nav pietiekami garš")]
        [DisplayName("Apraksts")]
        public string Description { get; set; } = string.Empty;
    }
}