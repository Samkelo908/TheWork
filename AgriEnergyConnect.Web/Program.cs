using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using System.Net.Http.Headers;

namespace AgriEnergyConnect.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllersWithViews();

            // Log the API Base URL to confirm it's loaded correctly
            Console.WriteLine("API Base URL: " + builder.Configuration["ApiSettings:BaseUrl"]);

            // Configure named HttpClient for general API communication
            builder.Services.AddHttpClient("AgriEnergyAPI", client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]);
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            builder.Services.AddHttpContextAccessor();

            // Add session for token storage
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Persist data protection keys to shared volume for session cookie encryption
            builder.Services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(@"/root/.aspnet/DataProtection-Keys"))
                .SetApplicationName("AgriEnergyConnectApp");

            // Add authentication with cookie scheme
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/AccessDenied"; // Added access denied path
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Added session timeout
                });

            builder.Services.AddAuthorization();

            // Configure AuthService HttpClient
            builder.Services.AddHttpClient<AuthService>(client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]);
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            builder.Services.AddHttpClient<EmployeeService>(client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]);
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromSeconds(30);
            });


            // Add ProductService with HttpClient configuration
            builder.Services.AddHttpClient<ProductService>(client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]);
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            // Add session middleware before authentication
            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}