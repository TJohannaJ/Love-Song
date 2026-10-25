using Backend.Domain.DTOs;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Backend.IntegrationTests.Controllers
{
    public class LikesControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public LikesControllerTests(CustomWebApplicationFactory factory)
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
        public async Task GetLikedYou_ReturnsOk()
        {
            var response = await _client.GetAsync("/likes/likedYou");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetLikes_ReturnsOk()
        {
            var response = await _client.GetAsync("/likes/likes");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetDislikes_ReturnsOk()
        {
            var response = await _client.GetAsync("/likes/dislikes");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task AcceptLike_MissingUserId_ReturnsBadRequest()
        {
            var request = new RequestDTO { RequestedUserId = "" };
            var response = await _client.PostAsJsonAsync("/likes/acceptLike", request);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task AcceptLike_UserNotInDatabase_ReturnsBadRequest()
        {
            var request = new RequestDTO { RequestedUserId = "non-existent-user" };
            var response = await _client.PostAsJsonAsync("/likes/acceptLike", request);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DeclineLike_MissingUserId_ReturnsBadRequest()
        {
            var request = new RequestDTO { RequestedUserId = null };
            var response = await _client.PostAsJsonAsync("/likes/declineLike", request);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UndoLike_MissingUserId_ReturnsBadRequest()
        {
            var request = new RequestDTO { RequestedUserId = "" };
            var response = await _client.PostAsJsonAsync("/likes/undoLike", request);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UndoDislike_MissingUserId_ReturnsBadRequest()
        {
            var request = new RequestDTO { RequestedUserId = "" };
            var response = await _client.PostAsJsonAsync("/likes/undoDislike", request);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UnauthenticatedUser_ReturnsUnauthorized()
        {
            using var unauthClient = _factory.CreateClient();
            var response = await unauthClient.GetAsync("/likes/likedYou");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}