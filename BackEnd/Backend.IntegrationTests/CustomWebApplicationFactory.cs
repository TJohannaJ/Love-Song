using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Backend.Domain.Interfaces;
using Backend.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

[assembly: Xunit.CollectionBehavior(DisableTestParallelization = true)]

namespace Backend.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");
            builder.ConfigureServices(services =>
            {
                var descriptorsToRemove = services.Where(d => 
                    d.ServiceType == typeof(MyDbContext) || 
                    d.ServiceType == typeof(DbContextOptions<MyDbContext>) ||
                    d.ServiceType == typeof(DbContextOptions)).ToList();

                foreach (var descriptor in descriptorsToRemove)
                {
                    services.Remove(descriptor);
                }

                var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
                var baseConnectionString = configuration.GetConnectionString("TestConnection");

                var connectionStringBuilder = new SqlConnectionStringBuilder(baseConnectionString)
                {
                    InitialCatalog = $"LoveSongTests_{Guid.NewGuid()}"
                };

                services.AddDbContext<MyDbContext>(options =>
                {
                    options.UseSqlServer(connectionStringBuilder.ConnectionString);
                });

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<MyDbContext>();
                db.Database.Migrate();

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                    options.DefaultScheme = "Test";
                }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });

                services.AddSingleton<IBlobService>(new Mock<IBlobService>().Object);
                services.AddSingleton<ISpotifyService>(new Mock<ISpotifyService>().Object);
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    using var scope = Services.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<MyDbContext>();
                    db.Database.EnsureDeleted();
                }
                catch
                {
                    // Suppress if provider/scope is already disposed
                }
            }
            base.Dispose(disposing);
        }
    }
}