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
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using WebProject.Core.Interfaces;
using WebProject.Core.Interfaces.Services;
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
            });
            services.AddDbContext<AppDbContext>(optionsBuilder =>
            {
                #if USING_INMEMORYDB
                optionsBuilder.UseInMemoryDatabase("AppDb");
                #endif
                
            });
            services.AddMvc()
            .AddRazorOptions(options =>
            {
                
            })
            .AddRazorPagesOptions(options =>
            {
                // TODO: Finish implementing error pages
                options.Conventions.AddPageRoute("/Page400", "/Shared/Error/Status400");
                options.Conventions.AddPageRoute("/Page404","/Shared/Error/Status404");
                options.Conventions.AddPageRoute("/Page500","/Shared/Error/Status500");
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
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
            
            // TODO add bad response handlers redirect to error pages
            
            // Add authorization policies
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireUserLogin", c => c.RequireRole("User", "Admin"));
                options.AddPolicy("RequireAdminLogin", c => c.RequireRole("Admin"));
            });
            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => {
                    // Cookie settings
                    options.Cookie = new CookieBuilder
                    {
                        SameSite = SameSiteMode.Strict,
                        SecurePolicy = CookieSecurePolicy.Always,
                        IsEssential = true,
                        HttpOnly = true,
                        Name = "Authentication"
                    };
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/SignOut";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    options.SlidingExpiration = true;
                });
            services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");
            services.AddRazorPages();
            services.AddSingleton<IEmailSenderService, EmailSenderService>();
            services.AddScoped<IMessageSenderService, MessageSenderService>();
            services.AddScoped<IEntityRepository<ListingCategory>, EfRepository<ListingCategory>>();
            services.AddScoped<IEntityRepository<Listing>, EfRepository<Listing>>();
            services.AddScoped<IEntityRepository<Message>, EfRepository<Message>>();
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, AppDbContext appDb, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            appDb.Database.EnsureCreatedAsync().GetAwaiter().GetResult();
            InitializeAuthorization(appDb, userManager, roleManager).GetAwaiter().GetResult();
         
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            // Add standard css/js/lib static files
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "Images")),
                RequestPath = "/Images"
            });
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(options =>
            {
                options.MapRazorPages();
                options.MapDefaultControllerRoute();
            });
        }

        private static async Task InitializeAuthorization(AppDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
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
            user = await userManager.FindByEmailAsync("user@email.com");
            if (user is null)
            {
                await userManager.CreateAsync(new ApplicationUser() {
                    UserName = "user",
                    Email = "user@email.com",
                    EmailConfirmed = true
                }, "parole123");
                user = await userManager.FindByEmailAsync("user@email.com");
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}