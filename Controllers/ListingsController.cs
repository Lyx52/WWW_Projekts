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
}