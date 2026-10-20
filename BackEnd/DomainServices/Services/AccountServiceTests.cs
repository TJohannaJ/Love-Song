using Moq;
using Xunit;
using Backend.Domain.Services;
using Backend.Domain.Interfaces;
using Backend.Domain.Entities;
using Backend.Domain.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace Backend.Domain.Tests.Services
{
    public class AccountServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IConfiguration> _configMock;
        private readonly AccountService _sut;

        public AccountServiceTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _configMock = new Mock<IConfiguration>();

            _configMock.Setup(c => c["JWT:Secret"]).Returns("SuperSecretKeyThatIsAtLeast32CharactersLong123!");
            _configMock.Setup(c => c["JWT:ValidIssuer"]).Returns("ValidIssuer");
            _configMock.Setup(c => c["JWT:ValidAudience"]).Returns("ValidAudience");

            _sut = new AccountService(_userRepoMock.Object, _configMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_CallsRepositoryAndReturnsResult()
        {
            var model = new RegisterDTO 
            { 
                Email = "test@test.com", 
                FullName = "Test User", 
                Password = "Password123!" 
            };
            var expectedResult = IdentityResult.Success;

            _userRepoMock.Setup(repo => repo.CreateAsync(It.IsAny<User>(), model.Password, "token"))
                         .ReturnsAsync(expectedResult);

            var result = await _sut.RegisterAsync(model);

            Assert.Equal(expectedResult, result);
            _userRepoMock.Verify(repo => repo.CreateAsync(It.Is<User>(u => 
                u.Email == model.Email && 
                u.FullName == model.FullName &&
                u.PictureUrl == "https://lovesongpics.blob.core.windows.net/images/profile_placeholder.png"
            ), model.Password, "token"), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_UserNotFound_ReturnsNull()
        {
            var model = new LoginDTO { Email = "test@test.com", Password = "Password123!" };
            _userRepoMock.Setup(repo => repo.FindByEmailAsync(model.Email)).ReturnsAsync((User)null);

            var result = await _sut.LoginAsync(model);

            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_WrongPassword_ReturnsNull()
        {
            var model = new LoginDTO { Email = "test@test.com", Password = "WrongPassword!" };
            var user = new User { Email = model.Email };
            _userRepoMock.Setup(repo => repo.FindByEmailAsync(model.Email)).ReturnsAsync(user);
            _userRepoMock.Setup(repo => repo.CheckPasswordAsync(user, model.Password)).ReturnsAsync(false);

            var result = await _sut.LoginAsync(model);

            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsTokenObject()
        {
            var model = new LoginDTO { Email = "test@test.com", Password = "Password123!" };
            var user = new User { Id = "user1", Email = model.Email, FullName = "Test User" };
            
            _userRepoMock.Setup(repo => repo.FindByEmailAsync(model.Email)).ReturnsAsync(user);
            _userRepoMock.Setup(repo => repo.CheckPasswordAsync(user, model.Password)).ReturnsAsync(true);

            var result = await _sut.LoginAsync(model);

            Assert.NotNull(result);
            _userRepoMock.Verify(repo => repo.SetRefreshTokenAsync(user, It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
        }

        [Fact]
        public async Task RefreshTokenAsync_TokenDTO_EmptyToken_ReturnsNull()
        {
            var model = new TokenDTO { RefreshToken = "" };

            var result = await _sut.RefreshTokenAsync(model);

            Assert.Null(result);
        }

        [Fact]
        public async Task RefreshTokenAsync_TokenDTO_InvalidToken_ReturnsNull()
        {
            var model = new TokenDTO { RefreshToken = "invalid_token" };
            _userRepoMock.Setup(repo => repo.FindByRefreshTokenAsync(model.RefreshToken)).ReturnsAsync((User)null);

            var result = await _sut.RefreshTokenAsync(model);

            Assert.Null(result);
        }

        [Fact]
        public async Task RefreshTokenAsync_TokenDTO_ValidToken_ReturnsNewTokens()
        {
            var model = new TokenDTO { RefreshToken = "valid_token" };
            var user = new User { Id = "user1", Email = "test@test.com" };
            
            _userRepoMock.Setup(repo => repo.FindByRefreshTokenAsync(model.RefreshToken)).ReturnsAsync(user);

            var result = await _sut.RefreshTokenAsync(model);

            Assert.NotNull(result);
            _userRepoMock.Verify(repo => repo.SetRefreshTokenAsync(user, It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
        }
        
        [Fact]
        public async Task RefreshTokenAsync_TokenRequestDTO_InvalidToken_ReturnsNull()
        {
            var model = new TokenRequestDTO { RefreshToken = "invalid_token" };
            _userRepoMock.Setup(repo => repo.FindByRefreshTokenAsync(model.RefreshToken)).ReturnsAsync((User)null);

            var result = await _sut.RefreshTokenAsync(model);

            Assert.Null(result);
        }

        [Fact]
        public async Task RefreshTokenAsync_TokenRequestDTO_ValidToken_ReturnsNewTokens()
        {
            var model = new TokenRequestDTO { RefreshToken = "valid_token" };
            var user = new User { Id = "user1", Email = "test@test.com" };
            
            _userRepoMock.Setup(repo => repo.FindByRefreshTokenAsync(model.RefreshToken)).ReturnsAsync(user);

            var result = await _sut.RefreshTokenAsync(model);

            Assert.NotNull(result);
            _userRepoMock.Verify(repo => repo.SetRefreshTokenAsync(user, It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
        }
    }
}