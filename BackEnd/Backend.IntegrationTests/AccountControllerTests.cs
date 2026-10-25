using Backend.Domain.DTOs;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Backend.IntegrationTests.Controllers
{
    public class AccountControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public AccountControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Register_ValidModel_ReturnsOk()
        {
            var registerDto = new RegisterDTO 
            { 
                Email = $"test_{Guid.NewGuid()}@example.com", // Egyedi email a futtatásokhoz
                Password = "Password123!", 
                FullName = "Test User",
                City = "Budapest",
                Gender = "Male",
                BirthDate = new DateTime(2000, 1, 1)
            };

            var response = await _client.PostAsJsonAsync("/account/register", registerDto);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            var loginDto = new LoginDTO 
            { 
                Email = "wrong@example.com", 
                Password = "WrongPassword!" 
            };

            var response = await _client.PostAsJsonAsync("/account/login", loginDto);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOk()
        {
            // 1. Létrehozunk egy usert a teszthez
            var email = $"login_{Guid.NewGuid()}@example.com";
            var password = "Password123!";
            await _client.PostAsJsonAsync("/account/register", new RegisterDTO 
            { 
                Email = email, 
                Password = password, 
                FullName = "Login User",
                City = "Budapest",
                Gender = "Male",
                BirthDate = new DateTime(2000, 1, 1)
            });

            // 2. Megpróbálunk belépni vele
            var loginDto = new LoginDTO { Email = email, Password = password };
            var response = await _client.PostAsJsonAsync("/account/login", loginDto);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Refresh_InvalidToken_ReturnsUnauthorized()
        {
            var tokenDto = new TokenRequestDTO 
            { 
                AccessToken = "invalid_access",
                RefreshToken = "invalid_refresh" 
            };

            var response = await _client.PostAsJsonAsync("/account/refresh-token", tokenDto);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}