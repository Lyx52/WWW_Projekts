using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using WebProject.Core.Interfaces;
using WebProject.Core.Models;

namespace WebProject.Pages.Listings;

public class Create : PageModel
{
    private static Random _random = new Random();
    private static string[] _acceptedFileExtensions = { "png", "jpg" };
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEntityRepository<ListingImage> _imageRepository;
    private readonly ILogger<Create> _logger;
    private readonly IHostEnvironment _environment;
    [TempData] 
    public string Images { get; set; }
    
    [BindProperty] 
    public CreateListingInputModel CreateListingInput { get; set; }
    
    [BindProperty]
    public UploadImageInputModel UploadImageInput { get; set; }

    public List<string> ImageList => string.IsNullOrEmpty(Images) ? new List<string>() : Images.Split(';').Where(i => !string.IsNullOrEmpty(i)).ToList();
    public Create(IHostEnvironment environment, ILogger<Create> logger, UserManager<ApplicationUser> userManager, IEntityRepository<ListingImage> imageRepository)
    {
        _userManager = userManager;
        _imageRepository = imageRepository;
        _logger = logger;
        _environment = environment;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        TempData.Clear();
        return Page();
    }
    public async Task<IActionResult> OnPostRemoveImageAsync(string? removeId)
    {
        TempData.Keep("Images");
        if (string.IsNullOrEmpty(removeId) || !Int32.TryParse(removeId, out int imgId))
        {
            ModelState.AddModelError("UploadImageInput.ImageFile", "Nav izvēlēts attēls!");
            return Page();
        }
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return Unauthorized();
        var img = await _imageRepository.AsQueryable()
            .Include(i => i.CreatedBy)
            .Where(i => i.Id == imgId && i.CreatedBy == user).FirstOrDefaultAsync();
        if (img is null)
        {
            ModelState.AddModelError("UploadImageInput.ImageFile", "Nevar izdzēst attēlu!");
            return Page();
        }

        await _imageRepository.Remove(img);
        try
        {
            // TODO: Remove from filesystem
            // Directory.Delete(Path.Combine(_environment.ContentRootPath, img.FilePath));
        }
        catch (IOException e)
        {
            _logger.LogError("Caught exception while trying to delete image {String}", e.Message);
        }

        Images = string.Join(';', Images.Split(';').Where(r => !r.EndsWith($"id:{removeId}")));
        return Page();
    }
    public async Task<IActionResult> OnPostUploadImageAsync()
    {
        TempData.Keep("Images");
        if (ModelState.GetFieldValidationState("UploadImageInput") == ModelValidationState.Valid)
        {
            if (!_acceptedFileExtensions.Any((ext) => UploadImageInput.ImageFile.FileName.EndsWith(ext)))
            {
                ModelState.AddModelError("UploadImageInput.ImageFile", "Attēla formāts nav atbalstīts!");
                return Page();
            }
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return Unauthorized();
            
            using (var imgStream = new MemoryStream())
            {
                await UploadImageInput.ImageFile.CopyToAsync(imgStream);
                imgStream.Seek(0, SeekOrigin.Begin);
                var image = await Image.LoadAsync(imgStream);
                double aspect = (double)image.Width / (double)image.Height;
                
                // Izfiltrējam bildes kurām aspekta attiecība ir zem 4:3 un virs 16:9
                if ((aspect) < 1.3D || (aspect) > 1.8D)
                {
                    ModelState.AddModelError("", "Faila izšķirtspēja neatbist prasībām!");
                    return Page();
                }

                string newFileName = $"{GetRandomFileName()}.jpg";
                image.Save(Path.Combine(_environment.ContentRootPath, "Images", newFileName), new JpegEncoder());
                var img = new ListingImage
                {
                    FilePath = $"/Images/{newFileName}", IsUsed = false, CreatedBy = user, Created = DateTime.UtcNow
                };
                await _imageRepository.Add(img);
                Images += $"{UploadImageInput.ImageFile.FileName}-id:{img.Id};";
            }
        }

        return Page();
    }
    public async Task<IActionResult> OnPostCreateListingAsync()
    {
        if (ModelState.IsValid)
        {
            
        }

        return Page();
    }
    
    public class UploadImageInputModel
    {
        [Required(ErrorMessage = "Attēls ir nepieciešams!")]
        public IFormFile ImageFile { get; set; }
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