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
                new () { Id = -1, Name = "Uncategorized" }
            });
        base.OnModelCreating(modelBuilder);
    }
    
}