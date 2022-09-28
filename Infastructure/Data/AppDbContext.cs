using Microsoft.EntityFrameworkCore;
using WebProject.Core.Models;

namespace WebProject.Infastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<Listing> Listings { get; set; }
    public DbSet<ListingCategory> Categories { get; set; }
    public DbSet<ListingImage> Images { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ListingCategory>()
            .HasData(new List<ListingCategory>()
            {
                new ListingCategory() { Id = -1, Name = "Category 1" },
                new ListingCategory() { Id = -2, Name = "Category 2" },
                new ListingCategory() { Id = -3, Name = "Category 3" }
            });
        var listings = new List<Listing>();
        for (int i = 1; i < 50; i++)
        {
            if (i % 2 == 0)
            {
                listings.Add(new Listing()
                {
                    Id = -i, CategoryId = -1, Title = $"Test{i}",
                    Description = "This is a wider card with supporting text below as a natural lead-in to additional content. This content is a little bit longer. This is a wider card with supporting text below as a natural lead-in to additional content. This content is a little bit longer. This is a wider card with supporting text below as a natural lead-in to additional content. This content is a little bit longer. This is a wider card with supporting text below as a natural lead-in to additional content. This content is a little bit longer.", 
                    Created = DateTime.UtcNow,
                    Price = Random.Shared.NextSingle() * 50
                });
                continue;
            }

            listings.Add(new Listing()
            {
                Id = -i, CategoryId = -1, Title = $"Test{i}",
                Description = "This is a wider card with supporting text below as a natural lead-in to additional content. This content is a little bit longer.",
                Created = DateTime.MinValue
            });
        }

        modelBuilder.Entity<Listing>()
            .HasData(listings);
        var images = new List<ListingImage>();
        int imgId = 1;
        for (int i = 1; i < 50; i++)
        {
            if (i % 2 == 0)
            {
                images.Add(
                    new ListingImage()
                    {
                        Id = -(imgId++), FilePath = "/Images/test2.jpg", ListingId = -i
                    });
                images.Add(
                    new ListingImage()
                    {
                        Id = -(imgId++), FilePath = "/Images/test.jpg", ListingId = -i
                    });
                images.Add(
                    new ListingImage()
                    {
                        Id = -(imgId++), FilePath = "/Images/test3.jpg", ListingId = -i
                    });
                continue;
            }

            images.Add(
                new ListingImage()
                {
                    Id = -(imgId++), FilePath = "/Images/test.jpg", ListingId = -i
                });

        }
        modelBuilder.Entity<ListingImage>()
            .HasData(images);
        base.OnModelCreating(modelBuilder);
    }
    
}