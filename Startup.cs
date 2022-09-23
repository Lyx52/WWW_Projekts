#define USING_INMEMORYDB
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Components.Server;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using WebProject.Core.Interfaces;
using WebProject.Core.Models;
using WebProject.Infastructure.Data;
using WebProject.Infastructure.Services;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IWebHostEnvironment;

namespace WebProject
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(optionsBuilder =>
            {
                #if USING_INMEMORYDB
                optionsBuilder.UseInMemoryDatabase("AppDb");
                #endif
            });
            services.AddDbContext<UserDbContext>(optionsBuilder =>
            {
                #if USING_INMEMORYDB
                optionsBuilder.UseInMemoryDatabase("UserDb");
                #endif
            });
            services.AddMvc()
            .AddRazorOptions(options =>
            {
                
            })
            .AddRazorPagesOptions(options =>
            {
                options.Conventions.AddPageRoute("/Pages/Shared/Error", "/Error");
                // TODO: Finish implementing error pages
                options.Conventions.AddPageRoute("/Pages/Shared/Error/Status400", "/400");
                options.Conventions.AddPageRoute("/Pages/Shared/Error/Status404", "/404");
                options.Conventions.AddPageRoute("/Pages/Shared/Error/Status500", "/500");
            });
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // TODO: Proper password configuration...
                options.Password.RequiredLength = 6;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            })
            .AddEntityFrameworkStores<UserDbContext>()
            .AddDefaultTokenProviders();

            // Add authorization policies
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireUserLogin", c => c.RequireRole("User", "Admin"));
                options.AddPolicy("RequireAdminLogin", c => c.RequireRole("Admin"));
            });
            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
            });
            services.AddRazorPages();
            services.AddSingleton<IEmailSenderService, EmailSenderService>();
            //services.AddScoped<IRepository<Zombie>, EfRepository<Zombie>>();
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, UserDbContext userDb, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            InitializeAuthorization(userDb, userManager, roleManager).GetAwaiter().GetResult();
         
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(options =>
            {
                options.MapRazorPages();
                options.MapDefaultControllerRoute();
            });
        }

        private static async Task InitializeAuthorization(UserDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await db.Database.EnsureCreatedAsync();
            
            // Izveidojam lietotāja konta tipu
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }
            // Izveidojam administratora konta tipu
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            // Izveidojam testa kontus
            var user = await userManager.FindByEmailAsync("admin@email.com");
            if (user is null)
            {
                await userManager.CreateAsync(new ApplicationUser() {
                    UserName = "admin",
                    Email = "admin@email.com",
                    EmailConfirmed = true
                }, "parole123");
                user = await userManager.FindByEmailAsync("admin@email.com");
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}