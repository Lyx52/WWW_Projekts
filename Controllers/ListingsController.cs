using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using WebProject.Core.Interfaces;
using WebProject.Core.Models;
using WebProject.Infastructure.Services;

namespace WebProject.Controllers;

[ApiController]
[Route("Listings")]
public class ListingsController : Controller
{
    private static Random _random = new Random();
    private static string[] AcceptedFileExtensions = { "png", "jpg" };
    private readonly ILogger<ListingsController> _logger;
    private readonly IEntityRepository<ListingImage> _imageRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    public ListingsController(ILogger<ListingsController> logger, UserManager<ApplicationUser> userManager, IEntityRepository<ListingImage> imageRepository)
    {
        _imageRepository = imageRepository;
        _userManager = userManager;
        _logger = logger;
    }
    public IActionResult Index()
    {
         return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    [Route("UploadFile")]
    public async Task<IActionResult> UploadFile(IFormFile? imgUpload)
    {
        if (imgUpload is null || !AcceptedFileExtensions.Any((ext) => imgUpload.FileName.EndsWith(ext)))
            return new JsonResult(new { success = false, message = "Fails nav atbalstīts!" });
        // Pārbaudam vai lietotājs ir ieloggojies
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return BadRequest();

        int imgId = -1;
        try
        {
            using (var imgStream = new MemoryStream())
            {
                await imgUpload.CopyToAsync(imgStream);
                imgStream.Seek(0, SeekOrigin.Begin);
                var image = await Image.LoadAsync(imgStream);
                double aspect = (double)image.Width / (double)image.Height;
                // Izfiltrējam bildes kurām aspekta attiecība ir zem 4:3 un virs 16:9
                if ((aspect) < 1.3D || (aspect) > 1.8D)
                    return new JsonResult(new { success = false, message = "Faila izšķirtspēja neatbist prasībām!" });
                string newFileName = $"{GetRandomFileName()}.jpg";
                image.Save(Path.Combine(Directory.GetCurrentDirectory(), "Images", newFileName), new JpegEncoder());
                var entity = new ListingImage
                {
                    FilePath = $"/Images/{newFileName}", IsUsed = false, CreatedBy = user, Created = DateTime.UtcNow
                };
                await _imageRepository.Add(entity);
                imgId = entity.Id;
            }
        }
        catch (Exception e)
        {
            _logger.LogError("Caught error while trying to upload image {String}", e.Message);
            return new JsonResult(new { success = false, message = "Neizdevās augšupielādēt failu!" });
        }
        return new JsonResult(new { success = true, imageId = DataEncoderService.Encode(imgId) });
    }

    private static string GetRandomFileName()
    {
        Span<char> chars = stackalloc char[8];
        for (int i = 0; i < chars.Length; i++)
        {
            chars[i] = (char)(_random.NextSingle() >= 0.5 ? _random.Next(65, 91) : _random.Next(97, 123));
        }
        return new string(chars);
    }
}