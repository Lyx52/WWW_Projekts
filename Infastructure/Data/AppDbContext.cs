using Microsoft.EntityFrameworkCore;
using WebProject.Core.Models;

namespace WebProject.Infastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<Listing> Listings { get; set; }
    public DbSet<ListingCategory> Categories { get; set; }
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
        // modelBuilder.Entity<ListingCategory>()
        //     .HasMany<ListingCategory>(c => c.SubCategories).WithMany(c => c.SubCategories);
        base.OnModelCreating(modelBuilder);
    }
    
}