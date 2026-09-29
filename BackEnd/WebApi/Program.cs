using Backend.Domain.Interfaces;
using Backend.Domain.Services;
using Backend.Infrastructure.DBEntities;
using Backend.Infrastructure.Persistence;
using Backend.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Backend.Infrastructure.Services;
using Resend;

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
                    policy.WithOrigins("https://localhost:5175", "http://localhost:5175") 
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials(); // Ez kell a SignalR-hez
                });
        });

        builder.Services.AddHttpContextAccessor();
       
        builder.Services.AddOptions();
        builder.Services.AddHttpClient<IResend, ResendClient>();
        builder.Services.Configure<ResendClientOptions>(o =>
        {
            o.ApiToken = builder.Configuration["Resend:ApiKey"]!;
        });
        builder.Services.AddTransient<IEmailService, EmailService>();
        
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ILikeRepository, LikeRepository>();
        builder.Services.AddScoped<IDislikeRepository, DislikeRepository>();
        builder.Services.AddScoped<IMessageRepository, MessageRepository>();
        builder.Services.AddScoped<ITokenRepository, TokenRepository>();
        builder.Services.AddScoped<ISongRepository, SongRepository>();
        builder.Services.AddScoped<IUserPreferenceRepository, UserPreferenceRepository>();

        builder.Services.AddScoped<MatchService>();
        builder.Services.AddScoped<DiscoveryService>();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<ChatService>();
        builder.Services.AddScoped<AccountService>();
        builder.Services.AddScoped<SpotifyService>();
        builder.Services.AddScoped<AccountService>();

        builder.Services.AddHttpClient();

        builder.Services.AddHttpClient<EssentiaService>();
        builder.Services.AddHttpClient<SpotifyService>();

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
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chathub"))
                    {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
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
        app.MapHub<ChatHub>("/chathub");
        app.Run();

    }

}