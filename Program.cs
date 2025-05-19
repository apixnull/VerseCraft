using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using VerseCraft.Data;
using VerseCraft.Services;
using DotNetEnv;


namespace VerseCraft
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            Env.Load();



            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Auth/Login";
                    options.LogoutPath = "/Auth/Logout";
                    options.ExpireTimeSpan = TimeSpan.FromDays(7);

                    options.Events = new CookieAuthenticationEvents
                    {
                        OnRedirectToLogin = context =>
                        {
                            if (context.Request.Path.StartsWithSegments("/admin"))
                            {
                                context.Response.Redirect("/Auth/NotFoundPage");
                                return Task.CompletedTask;
                            }

                            if (context.Request.Path.StartsWithSegments("/UserDashboard"))
                            {
                                context.Response.Redirect("/Home/Index?message=You need to login first to access this page");
                                return Task.CompletedTask;
                            }

                            context.Response.Redirect("/Auth/Login?message=You need to login first to access this page");
                            return Task.CompletedTask;
                        },
                        OnRedirectToAccessDenied = context =>
                        {
                           

                            context.Response.Redirect("/Home/Index?message=You are not authorized to access admin pages");
                            return Task.CompletedTask;
                        }
                    };
                });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy =>
                    policy.RequireRole("Admin"));
            });

            builder.Services.AddSingleton<EmailService>();

            builder.Services.AddControllersWithViews();

            // 1️⃣ Register DbContext with SQLite
            builder.Services.AddDbContext<VerseCraftDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
            );

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
