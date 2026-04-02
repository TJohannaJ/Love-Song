using Backend.Domain.Entities;
using Backend.Infrastructure.DBEntities;
using Backend.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

public class Program
{
    public static void Main(string[] args)
    {    
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend",
                policy =>
                {
                    policy.WithOrigins("https://localhost:5175") 
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
        });

        builder.Services.AddScoped<Backend.Domain.Services.UserService>();
        builder.Services.AddScoped<Backend.Domain.Interfaces.IUserRepository, Backend.Infrastructure.Repositories.UserRepository>();

        builder.Services.AddScoped<Backend.Domain.Services.RecommendationService>();
        builder.Services.AddScoped<Backend.Domain.Interfaces.IRecommendationRepository, Backend.Infrastructure.Repositories.RecommendationRepository>();

        builder.Services.AddDbContext<MyDbContext>(opt =>
            opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddAuthorization();
        builder.Services.AddIdentityApiEndpoints<DBUser>()
            .AddEntityFrameworkStores<MyDbContext>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1");
            });
        }
        //app.MapIdentityApi<IdentityUser>();
        app.UseHttpsRedirection();

        app.UseCors("AllowFrontend");

        app.UseAuthorization();

        app.MapControllers();

        app.Run();

    }

}