using Microsoft.EntityFrameworkCore;

namespace WebProject.Infastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
}