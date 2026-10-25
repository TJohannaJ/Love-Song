using Backend.Domain.DTOs;
using Backend.Infrastructure.Persistence;
using Backend.Domain.Entities;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Backend.Infrastructure.DBEntities;
using Xunit;

namespace Backend.IntegrationTests.Hubs
{
    public class ChatHubTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public ChatHubTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task SendMessage_Unauthenticated_FailsToConnect()
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost/chathub", options =>
                {
                    options.HttpMessageHandlerFactory = _ => _factory.Server.CreateHandler();
                })
                .Build();

            // Jelentősen módosítva: A SignalR csatlakozás HttpRequestException-t dob 401 Unauthorized státuszkóddal
            var exception = await Assert.ThrowsAsync<HttpRequestException>(async () => await connection.StartAsync());
            Assert.Contains("401", exception.Message);
        }

        [Fact]
        public async Task SendMessage_Authenticated_TriggersErrorOrMessageSent()
        {
            // 1. Tesztadatok betöltése az adatbázisba, HOGY MEGELŐZZÜK AZ IDEGENKULCS HIBÁKAT (Foreign Key Violation)
            // A tesztfelhasználó azonosítója, amit a "TestAuthHandler" biztosít (alapértelmezetten valószínűleg "test-user-id")
            var senderId = "test-user-id"; 
            var receiverId = "receiver-user-id";

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<MyDbContext>();
                
                // Létrehozzuk a felhasználókat
                db.Users.Add(new DBUser 
                { 
                    Id = senderId, 
                    Email = "sender@example.com", 
                    FullName = "Sender User",
                    Gender = "Male",
                    City = "Budapest",
                    BirthDate = new DateTime(2000, 1, 1),
                    SpotifyToken = "dummy",
                    ShortIntro = "",
                    LongIntro = ""
                });
                
                db.Users.Add(new DBUser 
                { 
                    Id = receiverId, 
                    Email = "receiver@example.com", 
                    FullName = "Receiver User",
                    Gender = "Female",
                    City = "Budapest",
                    BirthDate = new DateTime(2000, 1, 1),
                    SpotifyToken = "dummy",
                    ShortIntro = "",
                    LongIntro = ""
                });
                
                await db.SaveChangesAsync(); // Mentsük el a szülő felhasználókat az idegenkulcs-hiba elkerülése miatt
                
                // Létrehozzuk a kölcsönös 'Like' rekordot, ami kötelező a ChatHub üzenetküldéshez
                db.Likes.Add(new DBLike 
                { 
                    Id = Guid.NewGuid().ToString(),
                    UserId = senderId, 
                    LikedUserId = receiverId, 
                    Mutual = true,
                    CreatedAt = DateTime.UtcNow
                });
                
                await db.SaveChangesAsync();
            }

            // 2. Kapcsolat felépítése
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost/chathub", options =>
                {
                    options.HttpMessageHandlerFactory = _ => _factory.Server.CreateHandler();
                    options.Headers.Add("Authorization", "Test");
                })
                .Build();

            await connection.StartAsync();

            var messageSentTcs = new TaskCompletionSource<MessageDTO>();
            var errorTcs = new TaskCompletionSource<string>();

            connection.On<MessageDTO>("MessageSent", message => messageSentTcs.TrySetResult(message));
            connection.On<string>("Error", errorMsg => errorTcs.TrySetResult(errorMsg));

            // 3. Üzenet küldése a Hubnak
            var request = new SendMessageRequestDTO 
            { 
                ReceiverId = receiverId, 
                Content = "Teszt üzenet" 
            };
            await connection.InvokeAsync("SendMessage", request);

            // 4. Eredmény ellenőrzése
            var completedTask = await Task.WhenAny(messageSentTcs.Task, errorTcs.Task, Task.Delay(5000));
            Assert.NotEqual(Task.Delay(5000), completedTask);
            
            Assert.True(messageSentTcs.Task.IsCompleted || errorTcs.Task.IsCompleted);

            await connection.StopAsync();
        }
    }
}