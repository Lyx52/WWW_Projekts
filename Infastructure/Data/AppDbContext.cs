using Microsoft.EntityFrameworkCore;
using WebProject.Core.Models;

namespace WebProject.Infastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<Listing> Listings;
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
}