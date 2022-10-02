using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebProject.Core.Models;

namespace WebProject.Pages.Listings;

public class Create : PageModel
{
    public Listing Input { get; set; } 
    // public async Task OnPostAsync()
    // {
    //     var file = Path.Combine(Directory.GetCurrentDirectory(), "uploads", Upload.FileName);
    //     using (var fileStream = new FileStream(file, FileMode.Create))
    //     {
    //         await Upload.CopyToAsync(fileStream);
    //     }
    // }
    // public async Task<IActionResult> OnAsyncPost()
    // {
    //     Console.WriteLine("test");
    //     return Page();
    // }
}