using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebProject.Core.Models;

namespace WebProject.Infastructure.Data;

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
            .HasMany<Message>(user => user.Messages).WithOne(m => m.Recipient);
        modelBuilder.Entity<ApplicationUser>()
            .HasMany<Listing>(user => user.Listings).WithOne(l => l.CreatedBy);
        modelBuilder.Entity<ListingCategory>()
            .HasData(new List<ListingCategory>()
            {
                new () { Id = -1, Name = "Uncategorized" }
            });
        base.OnModelCreating(modelBuilder);
    }
    
}