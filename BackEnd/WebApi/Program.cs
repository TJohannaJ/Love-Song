using Backend.Infrastructure.DBEntities;
using Backend.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

        builder.Services.AddHttpContextAccessor();
       
        builder.Services.AddScoped<Backend.Domain.Interfaces.IUserRepository, Backend.Infrastructure.Repositories.UserRepository>();
        builder.Services.AddScoped<Backend.Domain.Interfaces.ILikeRepository, Backend.Infrastructure.Repositories.LikeRepository>();
        builder.Services.AddScoped<Backend.Domain.Interfaces.IDislikeRepository, Backend.Infrastructure.Repositories.DislikeRepository>();
        builder.Services.AddScoped<Backend.Domain.Interfaces.IMessageRepository, Backend.Infrastructure.Repositories.MessageRepository>();

        builder.Services.AddScoped<Backend.Domain.Services.MatchService>();
        builder.Services.AddScoped<Backend.Domain.Services.DiscoveryService>();
        builder.Services.AddScoped<Backend.Domain.Services.UserService>();
        builder.Services.AddScoped<Backend.Domain.Services.ChatService>();

        builder.Services.AddSignalR();

        builder.Services.AddDbContext<MyDbContext>(opt =>
            opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                ValidAudience = builder.Configuration["JWT:ValidAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
            };
        });
        builder.Services.AddIdentityApiEndpoints<DBUser>()
            .AddEntityFrameworkStores<MyDbContext>();

        builder.Services.AddAuthorization();
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

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();

    }

}