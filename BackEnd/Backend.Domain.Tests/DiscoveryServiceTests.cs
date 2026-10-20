using Moq;
using Xunit;
using Backend.Domain.Services;
using Backend.Domain.Interfaces;
using Backend.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Backend.Domain.Tests.Services
{
    public class DiscoveryServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<ILikeRepository> _likeRepoMock;
        private readonly Mock<IDislikeRepository> _dislikeRepoMock;
        private readonly Mock<IUserPreferenceRepository> _preferenceRepoMock;
        private readonly DiscoveryService _sut;

        public DiscoveryServiceTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _likeRepoMock = new Mock<ILikeRepository>();
            _dislikeRepoMock = new Mock<IDislikeRepository>();
            _preferenceRepoMock = new Mock<IUserPreferenceRepository>();

            _sut = new DiscoveryService(
                _userRepoMock.Object,
                _likeRepoMock.Object,
                _dislikeRepoMock.Object,
                _preferenceRepoMock.Object
            );
        }

        [Fact]
        public async Task GetRecommendationsForUser_UserNotFound_ReturnsEmptyList()
        {
            var userId = "user1";
            _userRepoMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync((User)null);

            var result = await _sut.GetRecommendationsForUser(userId);

            Assert.Empty(result);
            _likeRepoMock.Verify(repo => repo.GetLikedUserIdsById(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GetRecommendationsForUser_UserFound_ReturnsRecommendations()
        {
            var userId = "user1";
            var currentUser = new User { Id = userId };
            var likedIds = new List<string> { "user2" };
            var dislikedIds = new List<string> { "user3" };
            var recommendations = new List<User> { new User { Id = "user4" } };

            _userRepoMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync(currentUser);
            _likeRepoMock.Setup(repo => repo.GetLikedUserIdsById(userId)).ReturnsAsync(likedIds);
            _dislikeRepoMock.Setup(repo => repo.GetDislikedUserIdsById(userId)).ReturnsAsync(dislikedIds);
            _userRepoMock.Setup(repo => repo.GetRecommendationsForUser(currentUser, likedIds, dislikedIds)).ReturnsAsync(recommendations);

            var result = await _sut.GetRecommendationsForUser(userId);

            Assert.Equal(recommendations, result);
        }

        [Fact]
        public async Task GetLikedYouForUser_UserNotFound_ReturnsEmptyList()
        {
            var userId = "user1";
            _userRepoMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync((User)null);

            var result = await _sut.GetLikedYouForUser(userId);

            Assert.Empty(result);
            _likeRepoMock.Verify(repo => repo.GetLikedYouUsersById(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GetLikedYouForUser_UserFound_ReturnsUsers()
        {
            var userId = "user1";
            var currentUser = new User { Id = userId };
            var likedYouIds = new List<string> { "user2", "user3" };
            var expectedUsers = new List<User> { new User { Id = "user2" }, new User { Id = "user3" } };

            _userRepoMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync(currentUser);
            _likeRepoMock.Setup(repo => repo.GetLikedYouUsersById(userId)).ReturnsAsync(likedYouIds);
            _userRepoMock.Setup(repo => repo.GetLikedYouForUser(currentUser, likedYouIds)).ReturnsAsync(expectedUsers);

            var result = await _sut.GetLikedYouForUser(userId);

            Assert.Equal(expectedUsers, result);
        }

        [Fact]
        public async Task GetLikesForUser_UserNotFound_ReturnsEmptyList()
        {
            var userId = "user1";
            _userRepoMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync((User)null);

            var result = await _sut.GetLikesForUser(userId);

            Assert.Empty(result);
            _likeRepoMock.Verify(repo => repo.GetLikedNotMutualUserIdsById(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GetLikesForUser_UserFound_FiltersDislikesAndReturnsUsers()
        {
            var userId = "user1";
            var currentUser = new User { Id = userId };
            var likedNotMutualIds = new List<string> { "user2", "user3", "user4" };
            var dislikedIds = new List<string> { "user3" };
            var filteredIds = new List<string> { "user2", "user4" };
            var expectedUsers = new List<User> { new User { Id = "user2" }, new User { Id = "user4" } };

            _userRepoMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync(currentUser);
            _likeRepoMock.Setup(repo => repo.GetLikedNotMutualUserIdsById(userId)).ReturnsAsync(likedNotMutualIds);
            _dislikeRepoMock.Setup(repo => repo.GetDislikedUserIdsById(userId)).ReturnsAsync(dislikedIds);
            
            _userRepoMock.Setup(repo => repo.GetLikesForUser(currentUser, It.Is<List<string>>(ids => ids.SequenceEqual(filteredIds))))
                         .ReturnsAsync(expectedUsers);

            var result = await _sut.GetLikesForUser(userId);

            Assert.Equal(expectedUsers, result);
        }

        [Fact]
        public async Task GetDislikesForUser_UserNotFound_ReturnsEmptyList()
        {
            var userId = "user1";
            _userRepoMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync((User)null);

            var result = await _sut.GetDislikesForUser(userId);

            Assert.Empty(result);
            _dislikeRepoMock.Verify(repo => repo.GetDislikedUserIdsById(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GetDislikesForUser_UserFound_ReturnsUsers()
        {
            var userId = "user1";
            var currentUser = new User { Id = userId };
            var dislikedIds = new List<string> { "user2" };
            var expectedUsers = new List<User> { new User { Id = "user2" } };

            _userRepoMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync(currentUser);
            _dislikeRepoMock.Setup(repo => repo.GetDislikedUserIdsById(userId)).ReturnsAsync(dislikedIds);
            _userRepoMock.Setup(repo => repo.GetDislikesForUser(currentUser, dislikedIds)).ReturnsAsync(expectedUsers);

            var result = await _sut.GetDislikesForUser(userId);

            Assert.Equal(expectedUsers, result);
        }

        [Fact]
        public async Task GetUserPreference_CallsRepository()
        {
            var userId = "user1";
            var expectedPreference = new UserPreference { UserId = userId };
            _preferenceRepoMock.Setup(repo => repo.GetByUserIdAsync(userId)).ReturnsAsync(expectedPreference);

            var result = await _sut.GetUserPreference(userId);

            Assert.Equal(expectedPreference, result);
            _preferenceRepoMock.Verify(repo => repo.GetByUserIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task UpdatePreferencePartialAsync_CallsRepository()
        {
            var userId = "user1";
            int? minAge = 20;
            int? maxAge = 30;
            string? gender = "Male";
            string? city = "Budapest";
            bool? commonSongOnly = true;

            await _sut.UpdatePreferencePartialAsync(userId, minAge, maxAge, gender, city, commonSongOnly);

            _preferenceRepoMock.Verify(repo => repo.UpdatePartialAsync(userId, minAge, maxAge, gender, city, commonSongOnly), Times.Once);
        }
    }
}