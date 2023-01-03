using Domain.Models;

namespace WebProject.Pages.Shared;

public class ImageGalleryModel
{
    public List<ListingImage> Images { get; set; }
    public int GalleryId { get; set; }
}