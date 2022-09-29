using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebProject.Pages.Shared;

namespace WebProject.Controllers;

public class MessageController : Controller
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> MarkAsRead([FromForm]MarkReadModel model)
    {
        return NoContent();
    }
}