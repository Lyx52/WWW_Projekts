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
using Domain.Interfaces;
using Domain.Interfaces.Services;
using Domain.Models;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
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

        // Servisu konfigurācija
        public void ConfigureServices(IServiceCollection services)
        {
            // Sīkdatņu konfigurācija
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddDbContextFactory<AppDbContext>(optionsBuilder =>
            {
                #if USING_INMEMORYDB
                optionsBuilder.UseInMemoryDatabase("AppDb");
                #else
                var env = Environment.GetEnvironmentVariables();
                optionsBuilder.UseNpgsql(
                    $"Host=postgres;Username={(env["DbUser"])};Password={(env["DbPass"])};Database={(env["AppDbName"])}",
                    opt => opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
                #endif    
            }, ServiceLifetime.Transient);

            services.AddMvc();
            // Blazor servera puses serviss
            services.AddServerSideBlazor().AddCircuitOptions(x => x.DetailedErrors = true);
            
            // Identitātes datubāze
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
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
            
            // Autorizācijas serviss un politikas
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireUserLogin", c => c.RequireRole("User", "Admin"));
                options.AddPolicy("RequireAdminLogin", c => c.RequireRole("Admin"));
            });
            // Autentifikācijas serviss un konfigurācija
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
            services.AddAuthorizationCore();
            // Formu autentifikācijas serviss
            services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");
            services.AddRazorPages();
            
            // E-pasta sūtīšanas serviss
            services.AddSingleton<IEmailSenderService, EmailSenderService>();
            
            // Ziņu apmaiņas serviss
            services.AddScoped<IMessageSenderService, MessageSenderService>();
            
            
            // Datubāžu modeļu servisi
            services.AddScoped<IEntityRepository<ListingCategory>, EfRepository<ListingCategory>>();
            services.AddScoped<IEntityRepository<Listing>, EfRepository<Listing>>();
            services.AddScoped<IEntityRepository<ListingImage>, EfRepository<ListingImage>>();
            services.AddScoped<IEntityRepository<Message>, EfRepository<Message>>();
        }
        
        public void Configure(AppDbContext db, IApplicationBuilder app, IHostingEnvironment env, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Izveidojam Images direktoriju
            if (!Directory.Exists("./Images"))
                Directory.CreateDirectory("./Images");
            InitializeAuthorization(db, userManager, roleManager).GetAwaiter().GetResult();
         
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStatusCodePagesWithReExecute("/Status");
            
            // Add standard css/js/lib static files
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "Images")),
                RequestPath = "/Images",
                OnPrepareResponse = ctx =>
                {
                    // Kešojam bildes
                    ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age={(60 * 60 * 24 * 7)}");
                }
            });
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCookiePolicy();
            
            app.UseEndpoints(options =>
            {
                options.MapBlazorHub();
                // Blazor nepieciešamie faili
                options.MapBlazorHub("~/Listings/_blazor");
                options.MapBlazorHub("~/Listings/Create/_blazor");
                options.MapBlazorHub("~/Listings/Edit/_blazor");
                options.MapRazorPages();
                options.MapDefaultControllerRoute();
            });
        }

        private static async Task InitializeAuthorization(AppDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            /*
             * Inicializācija kas domāta testa videi
             */
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
                    EmailConfirmed = true,
                    LockoutEnd = DateTimeOffset.MinValue.ToUniversalTime()
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
                    EmailConfirmed = true,
                    LockoutEnd = DateTimeOffset.MinValue.ToUniversalTime()
                }, "parole123");
                user = await userManager.FindByEmailAsync("user@email.com");
                await userManager.AddToRoleAsync(user, "Admin");
            }
            var categories = new List<string>
            {
                "Elektronika",
                "Datori",
                "Portatīvie datori"
            };
            var prevId = -1;
            foreach (var category in categories)
            {
                var index = categories.IndexOf(category);
                prevId = await db.AddCategory(category, index == 0 ? -1 : prevId);
            }

           
            await db.SaveChangesAsync();
            for (int i = 0; i < 100; i++)
            {
                await db.GenerateSampleListing();
            }
        }
    }
}