using Microsoft.AspNetCore.Mvc;
using System.Web;
namespace WebProject.Controllers;

[ApiController]
[Route("Listings")]
public class ListingsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [Route("UploadFile")]
    public async Task<IActionResult> UploadFile([FromForm]IFormFile? fileImg)
    {
        if (fileImg is null)
            return BadRequest();
        var file = Path.Combine(Directory.GetCurrentDirectory(), "Images", fileImg.FileName);
        using (var fileStream = new FileStream(file, FileMode.Create))
        {
            await fileImg.CopyToAsync(fileStream);
        }
        return NoContent();
        // try
        // {
        //     if (file.ContentLength > 0)
        //     {
        //         string _FileName = Path.GetFileName(file.FileName);
        //         string _path = Path.Combine(Server.MapPath("~/UploadedFiles"), _FileName);
        //         file.SaveAs(_path);
        //     }
        //     ViewBag.Message = "File Uploaded Successfully!!";
        //     return View();
        // }
        // catch
        // {
        //     ViewBag.Message = "File upload failed!!";
        //     return View();
        // }
    }
}