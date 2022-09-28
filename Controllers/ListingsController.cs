using Microsoft.AspNetCore.Mvc;

namespace WebProject.Controllers;

public class ListingsController : Controller
{
    public IActionResult Index(int? id)
    {
        return View();
    }
}