using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using project_manager.Data;
using project_manager.Models;
using project_manager.Services;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        builder.Services.AddScoped<IServiceProject, ServiceProject>();
        builder.Services.AddScoped<IServiceTask, ServiceTask>();

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.SignIn.RequireConfirmedEmail = false;
            options.Password.RequireDigit = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 4;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Password.RequiredUniqueChars = 0;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/Account/Logout";
            options.AccessDeniedPath = "/Account/AccessDenied";
        });

        builder.Services.AddControllersWithViews();

        var app = builder.Build();


        app.UseHttpsRedirection();
        app.UseStaticFiles(); 

        app.UseRouting();

        app.UseAuthentication(); 
        app.UseAuthorization();  

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Projects}/{action=Index}/{id?}"
        );

        app.Run();
    }
}