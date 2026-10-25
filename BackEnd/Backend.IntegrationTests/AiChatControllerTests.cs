using Backend.Domain.DTOs;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace Backend.IntegrationTests.Controllers
{
    public class AiChatControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public AiChatControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
        }

        [Fact]
        public async Task SendMessage_ValidRequest_ReturnsOk()
        {
            var request = new
            {
                History = new List<ChatMessageDTO>
                {
                    new ChatMessageDTO { Role = "user", Content = "Hello AI!" }
                }
            };

            var response = await _client.PostAsJsonAsync("/aichat/message", request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UnauthenticatedUser_ReturnsUnauthorized()
        {
            using var unauthClient = _factory.CreateClient();
            
            var request = new
            {
                History = new List<ChatMessageDTO>()
            };
            
            var response = await unauthClient.PostAsJsonAsync("/aichat/message", request);
            
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}