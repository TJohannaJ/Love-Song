using System.Net;
using System.Net.Http.Headers;
using Xunit;

namespace Backend.IntegrationTests.Controllers
{
    public class ChatControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public ChatControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
        }

        [Fact]
        public async Task GetMatches_ReturnsOk()
        {
            var response = await _client.GetAsync("/chat/matches");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetMatches_UnauthenticatedUser_ReturnsUnauthorized()
        {
            using var unauthClient = _factory.CreateClient();
            
            var response = await unauthClient.GetAsync("/chat/matches");
            
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetConversation_ReturnsOk()
        {
            var response = await _client.GetAsync("/chat/conversation/other-user-id");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetConversation_UnauthenticatedUser_ReturnsUnauthorized()
        {
            using var unauthClient = _factory.CreateClient();
            
            var response = await _client.GetAsync("/chat/conversation/other-user-id");
            
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}