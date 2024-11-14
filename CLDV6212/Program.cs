using Azure.Storage.Blobs;
using Azure.Storage.Files.Shares;
using Azure.Storage.Queues;
using CLDV6212.Controllers;
using CLDV6212.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CLDV6212
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;

            builder.Services.AddControllersWithViews();

            // Register HttpClient and Logging services correctly
            builder.Services.AddHttpClient();
            builder.Services.AddHttpClient<UserController>();
            builder.Services.AddLogging();

            // Add session services
            builder.Services.AddDistributedMemoryCache(); // For storing session in memory
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
                options.Cookie.HttpOnly = true; // Makes the cookie inaccessible to client-side scripts
                options.Cookie.IsEssential = true; // Mark session cookie as essential
            });

            // Register BlobService with the configuration
            builder.Services.AddSingleton(new BlobService(configuration.GetConnectionString("AzureStorage")));

            builder.Services.AddSingleton<TableStorageServices>(sp =>
            {
                var connectionString = configuration.GetConnectionString("AzureStorage");
                var tableName = "Users";
                return new TableStorageServices(connectionString, tableName);
            });

            builder.Services.AddSingleton<QueueService>(sp =>
            {
                var connectionString = configuration.GetConnectionString("AzureStorage");
                return new QueueService(connectionString, "birdshare");
            });

            builder.Services.AddSingleton<AzureFileShareService>(sp =>
            {
                var connectionString = configuration.GetConnectionString("AzureStorage");
                return new AzureFileShareService(connectionString, "abcfiles");
            });

            // Add authentication and cookie authentication
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                            .AddCookie(options =>
                            {
                                options.LoginPath = "/User/Login"; // Specify the login path
                                options.AccessDeniedPath = "/User/AccessDenied"; // Specify the access denied path
                            });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // Use authentication and authorization middleware
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRouting();

            // Enable session before authorization middleware
            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}