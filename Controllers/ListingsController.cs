using Microsoft.AspNetCore.Mvc;

namespace WebProject.Controllers;

public class ListingsController : Controller
{
    [HttpGet]
    public IActionResult Index(int? id)
    {
        return View();
    }
}