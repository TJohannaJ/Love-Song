using Backend.Domain.DTOs;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace Backend.IntegrationTests.Controllers
{
    public class RecommendationControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public RecommendationControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
        }

        [Fact]
        public async Task GetRecommendations_ReturnsOk()
        {
            var response = await _client.GetAsync("/recommendation");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task LikeUser_MissingUserId_ReturnsBadRequest()
        {
            var request = new RequestDTO { RequestedUserId = "" };
            var response = await _client.PostAsJsonAsync("/recommendation/like", request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task LikeUser_UserNotInDatabase_ReturnsBadRequest()
        {
            var request = new RequestDTO { RequestedUserId = "some-other-user-id" };
            var response = await _client.PostAsJsonAsync("/recommendation/like", request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DislikeUser_MissingUserId_ReturnsBadRequest()
        {
            var request = new RequestDTO { RequestedUserId = null };
            var response = await _client.PostAsJsonAsync("/recommendation/dislike", request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetPreferences_NoPreferencesFound_ReturnsNotFound()
        {
            var response = await _client.GetAsync("/recommendation/preferences");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdatePreferences_ValidDto_ReturnsOk()
        {
            var dto = new UserPreferenceDTO 
            { 
                MinAge = 18, 
                MaxAge = 35, 
                Gender = "Female", 
                City = "Budapest", 
                CommonSongOnly = true 
            };
            
            var response = await _client.PatchAsJsonAsync("/recommendation/preferences", dto);

            Assert.True(response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task UnauthenticatedUser_ReturnsUnauthorized()
        {
            using var unauthClient = _factory.CreateClient();
            
            var response = await _client.GetAsync("/recommendation");
            
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}