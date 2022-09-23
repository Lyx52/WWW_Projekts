using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebProject.Pages.Admin;

public class Dashboard : PageModel
{
    public string ReturnUrl { get; set; }
    public string Test { get; set; }
    public void OnGet()
    {
        Test = "HELLO!!@!#!3";
    }
}