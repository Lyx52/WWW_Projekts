using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebProject.Core.Models;

namespace WebProject.Infastructure.Data;

public class UserDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly ILogger<UserDbContext> _logger;
    public UserDbContext(DbContextOptions<UserDbContext> options, ILogger<UserDbContext> logger) : base(options)
    {
        _logger = logger;
    }
}