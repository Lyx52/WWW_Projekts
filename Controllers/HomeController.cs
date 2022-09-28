using Microsoft.AspNetCore.Mvc;

namespace WebProject.Controllers;

public class HomeController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}