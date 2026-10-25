using System.Net;
using System.Net.Http.Headers;
using Xunit;

namespace Backend.IntegrationTests.Controllers
{
    public class SpotifyControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public SpotifyControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
        }

        [Fact]
        public async Task GetLoginUrl_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/spotify/login-url");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Callback_ReturnsHtmlContent()
        {
            var response = await _client.GetAsync("/api/spotify/callback?code=test-code&state=test-state");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
        }

        [Fact]
        public async Task GetTopTracks_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/spotify/top-tracks");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UnauthenticatedUser_ReturnsUnauthorized()
        {
            using var unauthClient = _factory.CreateClient();
            
            var response = await unauthClient.GetAsync("/api/spotify/login-url");
            
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}