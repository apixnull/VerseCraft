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

            // 2️⃣ Register Authentication & Cookie Scheme
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Auth/Login";
                    options.LogoutPath = "/Auth/Logout";
                    options.ExpireTimeSpan = TimeSpan.FromDays(7);

                    // 👇 Add custom logic to pass message
                    options.Events = new CookieAuthenticationEvents
                    {
                        OnRedirectToLogin = context =>
                        {
                            if (context.Request.Path.StartsWithSegments("/admin"))
                            {
                                context.Response.Redirect("/Auth/NotFoundPage");
                                return Task.CompletedTask;
                            }

                            context.Response.Redirect("/Auth/Login");
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



            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // 1️⃣ Register DbContext with SQLite
            builder.Services.AddDbContext<VerseCraftDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // order matter here
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
