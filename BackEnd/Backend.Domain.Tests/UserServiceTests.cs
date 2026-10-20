using Moq;
using Xunit;
using Backend.Domain.Services;
using Backend.Domain.Interfaces;
using Backend.Domain.Entities;
using Backend.Domain.DTOs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Backend.Domain.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IAiBioGenerator> _aiBioGeneratorMock;
        private readonly UserService _sut;

        public UserServiceTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _aiBioGeneratorMock = new Mock<IAiBioGenerator>();

            _sut = new UserService(_userRepoMock.Object, _aiBioGeneratorMock.Object);
        }

        [Fact]
        public async Task GetUser_ReturnsUserFromRepository()
        {
            var userId = "user1";
            var expectedUser = new User { Id = userId };
            _userRepoMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync(expectedUser);

            var result = await _sut.GetUser(userId);

            Assert.Equal(expectedUser, result);
            _userRepoMock.Verify(repo => repo.GetUserById(userId), Times.Once);
        }

        [Fact]
        public async Task UpdateProfileAsync_CallsRepository()
        {
            var userId = "user1";
            var model = new UserDTO();

            await _sut.UpdateProfileAsync(userId, model);

            _userRepoMock.Verify(repo => repo.UpdateProfile(userId, model), Times.Once);
        }

        [Fact]
        public async Task GenerateBioWithAiAsync_UserNotFound_ThrowsException()
        {
            var userId = "user1";
            var dto = new GenerateBioDTO();
            _userRepoMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync((User)null);

            await Assert.ThrowsAsync<Exception>(() => _sut.GenerateBioWithAiAsync(userId, dto));

            _aiBioGeneratorMock.Verify(ai => ai.GenerateBothBiosAsync(It.IsAny<User>(), It.IsAny<GenerateBioDTO>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GenerateBioWithAiAsync_UserFound_ReturnsGeneratedBio()
        {
            var userId = "user1";
            var user = new User { Id = userId };
            var dto = new GenerateBioDTO();
            var expectedResult = new GeneratedBioResultDTO();
            var cancellationToken = new CancellationToken();

            _userRepoMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync(user);
            _aiBioGeneratorMock.Setup(ai => ai.GenerateBothBiosAsync(user, dto, cancellationToken)).ReturnsAsync(expectedResult);

            var result = await _sut.GenerateBioWithAiAsync(userId, dto, cancellationToken);

            Assert.Equal(expectedResult, result);
            _aiBioGeneratorMock.Verify(ai => ai.GenerateBothBiosAsync(user, dto, cancellationToken), Times.Once);
        }
    }
}