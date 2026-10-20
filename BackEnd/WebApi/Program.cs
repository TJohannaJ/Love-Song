using System.ClientModel;
using System.Text;
using Backend.Domain.Interfaces;
using Backend.Domain.Services;
using Backend.Infrastructure.DBEntities;
using Backend.Infrastructure.ExternalApis;
using Backend.Infrastructure.ExternalApis.Tools;
using Backend.Infrastructure.Persistence;
using Backend.Infrastructure.Repositories;
using Backend.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.IdentityModel.Tokens;                    
using OpenAI;
using OpenAI.Chat;
using Resend;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

public class Program
{
    public static void Main(string[] args)
    {    
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.        
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend",
                policy =>
                {
                    policy.WithOrigins("https://localhost:5175", "http://localhost:5175") 
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
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

        // --- AI SZOLGÁLTATÁS REGISZTRÁCIÓ (Groq API) ---
        builder.Services.AddSingleton<IChatClient>(sp =>
        {
            var apiKey = builder.Configuration["Groq:ApiKey"] 
                         ?? throw new InvalidOperationException("Groq API key hiányzik!");

            // Próbáld ki ezt az ultragyors modellt, ha a 70b-re nem ad hozzáférést a kulcsod:
            return new GroqChatClient(apiKey, "openai/gpt-oss-120b");
        });
        
        builder.Services.AddScoped<IAiBioGenerator, AiBioGenerator>();
        builder.Services.AddScoped<AppNavigationTool>();
        builder.Services.AddScoped<FAQTool>();
        builder.Services.AddScoped<SemanticRecommendationTool>();
        builder.Services.AddScoped<IAiChatBot, AiChatBot>();
        
        // -------------------------------------------------------------
        
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ILikeRepository, LikeRepository>();
        builder.Services.AddScoped<IDislikeRepository, DislikeRepository>();
        builder.Services.AddScoped<IMessageRepository, MessageRepository>();
        builder.Services.AddScoped<ITokenRepository, TokenRepository>();
        builder.Services.AddScoped<ISongRepository, SongRepository>();
        builder.Services.AddScoped<IUserPreferenceRepository, UserPreferenceRepository>();
        builder.Services.AddScoped<IBlobService, AzureBlobService>();

        builder.Services.AddScoped<MatchService>();
        builder.Services.AddScoped<DiscoveryService>();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<ChatService>();
        builder.Services.AddScoped<AccountService>();
        builder.Services.AddScoped<SpotifyService>();
        builder.Services.AddScoped<ChatBotService>();

        builder.Services.AddHttpClient();
        builder.Services.AddHttpClient<EssentiaService>();
        builder.Services.AddHttpClient<SpotifyService>();

        builder.Services.AddSignalR();

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("A 'DefaultConnection' kapcsolati karakterlánc nem található a konfigurációban!");
        }

        builder.Services.AddDbContext<MyDbContext>(opt =>
            opt.UseSqlServer(connectionString));
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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
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

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1");
            });
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowFrontend");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapHub<ChatHub>("/chathub");
        app.Run();
    }
}

// --- JAVÍTOTT GROQ CHAT CLIENT IMPLEMENTÁCIÓ ---
public class GroqChatClient : IChatClient
{
    private readonly ChatClient _chatClient;

    public GroqChatClient(string apiKey, string modelId = "llama-3.3-70b-versatile")
    {
        var openAiClient = new OpenAIClient(
            new ApiKeyCredential(apiKey),
            new OpenAIClientOptions { Endpoint = new Uri("https://api.groq.com/openai/v1") }
        );

        _chatClient = openAiClient.GetChatClient(modelId);
    }

    public async Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages, 
        ChatOptions? options = null, 
        CancellationToken cancellationToken = default)
    {
        var openAiMessages = messages.Select<ChatMessage, OpenAI.Chat.ChatMessage>(m => m.Role switch
        {
            ChatRole r when r == ChatRole.System => new SystemChatMessage(m.Text),
            ChatRole r when r == ChatRole.User => new UserChatMessage(m.Text),
            _ => new AssistantChatMessage(m.Text)
        }).ToList();

        ClientResult<ChatCompletion> completion = await _chatClient.CompleteChatAsync(openAiMessages, cancellationToken: cancellationToken);
        
        var replyText = completion.Value.Content.FirstOrDefault()?.Text ?? string.Empty;
        return new ChatResponse(new ChatMessage(ChatRole.Assistant, replyText));
    }

    public IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages, 
        ChatOptions? options = null, 
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public object? GetService(Type serviceType, object? serviceKey = null) => this;
    public void Dispose() { }
}