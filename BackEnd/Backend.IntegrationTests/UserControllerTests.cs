using Backend.Domain.DTOs;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Backend.IntegrationTests.Controllers
{
    public class UserControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public UserControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            var options = new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("http://localhost"),
                AllowAutoRedirect = false
            };
            
            _client = factory.CreateClient(options);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
        }

        [Fact]
        public async Task GetUser_UserNotFound_ReturnsNotFound()
        {
            var response = await _client.GetAsync("/user");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateProfile_UserNotInDatabase_ReturnsBadRequest()
        {
            var model = new UserDTO { FullName = "Teszt Elek", City = "Budapest" };
            var response = await _client.PatchAsJsonAsync("/user/updateProfile", model);
            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GenerateBio_UserNotInDatabase_ReturnsBadRequest()
        {
            var model = new GenerateBioDTO { Keywords = new List<string> { "music", "coffee" }, Tone = "funny" };
            var response = await _client.PostAsJsonAsync("/user/generateBio", model);
            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetUploadUrl_WithMockedBlobService_ReturnsNoContent()
        {
            var response = await _client.GetAsync("/user/get-upload-url?fileName=test.jpg&contentType=image/jpeg");
            
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task UnauthenticatedUser_ReturnsUnauthorized()
        {
            using var unauthClient = _factory.CreateClient();
            var response = await unauthClient.GetAsync("/user");
            
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}