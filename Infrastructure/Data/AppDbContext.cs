using Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data;
public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly ILogger<AppDbContext> _logger;
    
    public DbSet<Listing> Listings { get; set; }
    public DbSet<ListingCategory> Categories { get; set; }
    public DbSet<ListingImage> Images { get; set; }
    public DbSet<Message> Messages { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options, ILogger<AppDbContext> logger) : base(options)
    {
        _logger = logger;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationUser>()
            .HasMany<Message>(user => user.RecvMessages).WithOne(m => m.Recipient).HasForeignKey(m => m.RecipientKey);
        modelBuilder.Entity<ApplicationUser>()
            .HasMany<Message>(user => user.SentMessages).WithOne(m => m.CreatedBy).HasForeignKey(m => m.CreatedByKey);
        modelBuilder.Entity<ApplicationUser>()
            .HasMany<Listing>(user => user.Listings).WithOne(l => l.CreatedBy).HasForeignKey(l => l.CreatedByKey);

        modelBuilder.Entity<ListingImage>()
            .HasOne<Listing>(li => li.Listing).WithMany(li => li.Images).HasForeignKey(li => li.ListingId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Listing>()
            .HasMany<ListingImage>(l => l.Images).WithOne(li => li.Listing).HasForeignKey(li => li.ListingId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<ListingCategory>()
            .HasData(new List<ListingCategory>()
            {
                new () { Id = -1, Name = "Uncategorized" },
            });
        
        base.OnModelCreating(modelBuilder);
    }

    public async Task<int> AddCategory(string title, int parentId)
    {
        var parent = await this.Set<ListingCategory>().FirstOrDefaultAsync(c => c.Id == parentId);
        if (parent is null) return -1;
        var entity = await this.Set<ListingCategory>().AddAsync(new ListingCategory()
        {
            Name = title,
            ParentCategoryId = parentId,
            ParentCategory = parent
        });
        await this.SaveChangesAsync();
        return entity.Entity.Id;
    }

    public async Task<ListingImage> AddImage(string imageUrl)
    {
        var image = new ListingImage()
        {
            FilePath = imageUrl,
            OriginalName = $"image_{Random.Shared.NextInt64()}.jpg",
            Created = DateTime.Now
        };
        var entity = (await this.Set<ListingImage>().AddAsync(image)).Entity;
        await this.SaveChangesAsync();
        return entity;
    }
    public async Task GenerateSampleListing()
    {
        var category = await this.Set<ListingCategory>().FirstOrDefaultAsync(c => c.Name == "Portatīvie datori");
        var user = await this.Set<ApplicationUser>().FirstOrDefaultAsync(u => u.UserName == "admin");
        var imgsDir = (new DirectoryInfo("./Images")).GetFiles().ToList();
        var min = Random.Shared.Next(0, imgsDir.Count - 2);
        var images = new List<ListingImage>();
        foreach (var image in imgsDir.Skip(min))
        {
            images.Add(await AddImage($"/Images/{image.Name}"));
        }
        if (category is null || user is null) return;
        var listing = new Listing()
        {
            Description = "Lorem ipsum dolor sit amet, \nconsectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. \nUt enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea \n commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat \ncupidatat non proident, sunt in culpa qui officia deserunt \nmollit anim id est laborum.",
            Title = $"Test{Random.Shared.Next()}",
            Category = category,
            CategoryId = category.Id,
            Price = Random.Shared.NextSingle() * 200,
            CreatedBy = user,
            Created = DateTime.Now.AddMinutes(-Random.Shared.Next(0, 30)),
            CreatedByKey = user.Id,
            Images = images
        };
        await Set<Listing>().AddAsync(listing);
        await SaveChangesAsync();
    }
    
}