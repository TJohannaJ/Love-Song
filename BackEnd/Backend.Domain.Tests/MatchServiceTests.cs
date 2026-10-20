using Moq;
using Xunit;
using Backend.Domain.Services;
using Backend.Domain.Interfaces;
using Backend.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace Backend.Domain.Tests.Services
{
    public class MatchServiceTests
    {
        private readonly Mock<ILikeRepository> _likeRepoMock;
        private readonly Mock<IDislikeRepository> _dislikeRepoMock;
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly MatchService _sut;

        public MatchServiceTests()
        {
            _likeRepoMock = new Mock<ILikeRepository>();
            _dislikeRepoMock = new Mock<IDislikeRepository>();
            _userRepoMock = new Mock<IUserRepository>();
            _emailServiceMock = new Mock<IEmailService>();

            _sut = new MatchService(
                _likeRepoMock.Object,
                _dislikeRepoMock.Object,
                _userRepoMock.Object,
                _emailServiceMock.Object
            );
        }

        [Fact]
        public async Task LikeUser_ExistingLike_ReturnsEarly()
        {
            var userId = "user1";
            var likedUserId = "user2";
            _likeRepoMock.Setup(repo => repo.GetByUsers(userId, likedUserId)).ReturnsAsync(new Like());

            await _sut.LikeUser(userId, likedUserId);

            _dislikeRepoMock.Verify(repo => repo.UndoDislike(userId, likedUserId), Times.Once);
            _likeRepoMock.Verify(repo => repo.AddOrUpdateAsync(It.IsAny<Like>()), Times.Never);
        }

        [Fact]
        public async Task LikeUser_NotMutual_SavesLike()
        {
            var userId = "user1";
            var likedUserId = "user2";
            _likeRepoMock.Setup(repo => repo.GetByUsers(userId, likedUserId)).ReturnsAsync((Like)null);
            _likeRepoMock.Setup(repo => repo.GetByUsers(likedUserId, userId)).ReturnsAsync((Like)null);

            await _sut.LikeUser(userId, likedUserId);

            _likeRepoMock.Verify(repo => repo.AddOrUpdateAsync(It.Is<Like>(l => 
                l.UserId == userId && 
                l.LikedUserId == likedUserId && 
                !l.Mutual)), Times.Once);
        }

        [Fact]
        public async Task DislikeUser_ExistingDislike_ReturnsEarly()
        {
            var userId = "user1";
            var dislikedUserId = "user2";
            _dislikeRepoMock.Setup(repo => repo.GetByUsers(userId, dislikedUserId)).ReturnsAsync(new Dislike());

            await _sut.DislikeUser(userId, dislikedUserId);

            _dislikeRepoMock.Verify(repo => repo.AddOrUpdateAsync(It.IsAny<Dislike>()), Times.Never);
            _likeRepoMock.Verify(repo => repo.UndoLike(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task DislikeUser_NewDislike_SavesDislikeAndUndoesLike()
        {
            var userId = "user1";
            var dislikedUserId = "user2";
            _dislikeRepoMock.Setup(repo => repo.GetByUsers(userId, dislikedUserId)).ReturnsAsync((Dislike)null);

            await _sut.DislikeUser(userId, dislikedUserId);

            _dislikeRepoMock.Verify(repo => repo.AddOrUpdateAsync(It.Is<Dislike>(d => 
                d.UserId == userId && 
                d.DislikedUserId == dislikedUserId)), Times.Once);
            _likeRepoMock.Verify(repo => repo.UndoLike(userId, dislikedUserId), Times.Once);
        }

        [Fact]
        public async Task UndoLike_CallsRepository()
        {
            var userId = "user1";
            var likedUserId = "user2";

            await _sut.UndoLike(userId, likedUserId);

            _likeRepoMock.Verify(repo => repo.UndoLike(userId, likedUserId), Times.Once);
        }

        [Fact]
        public async Task UndoDislike_CallsRepository()
        {
            var userId = "user1";
            var dislikedUserId = "user2";

            await _sut.UndoDislike(userId, dislikedUserId);

            _dislikeRepoMock.Verify(repo => repo.UndoDislike(userId, dislikedUserId), Times.Once);
        }

        [Fact]
        public async Task GetMatchesForUserAsync_UserNotFound_ReturnsEmptyList()
        {
            var userId = "user1";
            _userRepoMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync((User)null);

            var result = await _sut.GetMatchesForUserAsync(userId);

            Assert.Empty(result);
            _likeRepoMock.Verify(repo => repo.GetLikedMutualUsersById(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GetMatchesForUserAsync_UserFound_ReturnsMatches()
        {
            var userId = "user1";
            var currentUser = new User { Id = userId };
            var mutualIds = new List<string> { "user2", "user3" };
            var matches = new List<User> { new User { Id = "user2" }, new User { Id = "user3" } };

            _userRepoMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync(currentUser);
            _likeRepoMock.Setup(repo => repo.GetLikedMutualUsersById(userId)).ReturnsAsync(mutualIds);
            _userRepoMock.Setup(repo => repo.GetMatchesForUser(currentUser, mutualIds)).ReturnsAsync(matches);

            var result = await _sut.GetMatchesForUserAsync(userId);

            Assert.Equal(matches, result);
        }
    }
}