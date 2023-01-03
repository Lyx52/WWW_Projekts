using Microsoft.AspNetCore.Mvc;

namespace WebProject.Controllers;

[ApiController]
[Route("Home")]
public class HomeController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}