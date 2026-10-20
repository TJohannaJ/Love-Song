using Moq;
using Xunit;
using Backend.Domain.Services;
using Backend.Domain.Interfaces;
using Backend.Domain.Entities;
using Backend.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace Backend.Domain.Tests.Services
{
    public class ChatServiceTests
    {
        private readonly Mock<IMessageRepository> _messageRepoMock;
        private readonly Mock<ILikeRepository> _likeRepoMock;
        private readonly ChatService _sut;

        public ChatServiceTests()
        {
            _messageRepoMock = new Mock<IMessageRepository>();
            _likeRepoMock = new Mock<ILikeRepository>();

            _sut = new ChatService(_messageRepoMock.Object, _likeRepoMock.Object);
        }

        [Fact]
        public async Task SaveMessageAsync_InvalidLikeIds_ThrowsException()
        {
            var request = new SendMessageRequestDTO
            {
                SenderId = "user1",
                ReceiverId = "user2",
                Content = "Hello"
            };

            var incompleteLikes = new List<Like> 
            { 
                new Like { UserId = "user1", Id = "like1" }
            };

            _likeRepoMock.Setup(repo => repo.GetMatchPair(request.SenderId, request.ReceiverId))
                         .ReturnsAsync(incompleteLikes);

            await Assert.ThrowsAsync<Exception>(() => _sut.SaveMessageAsync(request));
            _messageRepoMock.Verify(repo => repo.AddAsync(It.IsAny<Message>()), Times.Never);
        }

        [Fact]
        public async Task SaveMessageAsync_ValidRequest_SavesAndReturnsMessage()
        {
            var request = new SendMessageRequestDTO
            {
                SenderId = "user1",
                ReceiverId = "user2",
                Content = "Hello"
            };

            var validLikes = new List<Like>
            {
                new Like { UserId = "user1", Id = "like1" },
                new Like { UserId = "user2", Id = "like2" }
            };

            _likeRepoMock.Setup(repo => repo.GetMatchPair(request.SenderId, request.ReceiverId))
                         .ReturnsAsync(validLikes);

            var result = await _sut.SaveMessageAsync(request);

            Assert.NotNull(result);
            Assert.Equal(request.Content, result.Content);
            Assert.Equal(request.SenderId, result.SenderId);
            _messageRepoMock.Verify(repo => repo.AddAsync(It.Is<Message>(m => 
                m.SenderId == request.SenderId &&
                m.ReceiverId == request.ReceiverId &&
                m.Content == request.Content &&
                m.LikeId1 == "like1" &&
                m.LikeId2 == "like2"
            )), Times.Once);
        }

        [Fact]
        public async Task GetChatHistoryAsync_ReturnsMappedMessages()
        {
            var likeId = "like1";
            var messages = new List<Message>
            {
                new Message { Id = "msg1", Content = "Hi", SenderId = "user1", CreatedAt = DateTime.UtcNow }
            };

            _messageRepoMock.Setup(repo => repo.GetByLikeIdAsync(likeId)).ReturnsAsync(messages);

            var result = await _sut.GetChatHistoryAsync(likeId);

            Assert.Single(result);
            Assert.Equal(messages[0].Id, result.First().Id);
            Assert.Equal(messages[0].Content, result.First().Content);
        }

        [Fact]
        public async Task GetLastMessageBetweenUsersAsync_NoMatch_ReturnsNull()
        {
            var userId1 = "user1";
            var userId2 = "user2";
            var incompleteLikes = new List<Like> { new Like { Id = "like1" } };

            _likeRepoMock.Setup(repo => repo.GetMatchPair(userId1, userId2)).ReturnsAsync(incompleteLikes);

            var result = await _sut.GetLastMessageBetweenUsersAsync(userId1, userId2);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetLastMessageBetweenUsersAsync_NoMessages_ReturnsNull()
        {
            var userId1 = "user1";
            var userId2 = "user2";
            var validLikes = new List<Like> { new Like { Id = "like1" }, new Like { Id = "like2" } };

            _likeRepoMock.Setup(repo => repo.GetMatchPair(userId1, userId2)).ReturnsAsync(validLikes);
            _messageRepoMock.Setup(repo => repo.GetLastMessageByLikeIdsAsync(It.IsAny<List<string>>())).ReturnsAsync((Message)null);

            var result = await _sut.GetLastMessageBetweenUsersAsync(userId1, userId2);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetLastMessageBetweenUsersAsync_HasMessage_ReturnsMappedMessage()
        {
            var userId1 = "user1";
            var userId2 = "user2";
            var validLikes = new List<Like> { new Like { Id = "like1" }, new Like { Id = "like2" } };
            var lastMessage = new Message { Id = "msg1", Content = "Last one", SenderId = userId1, CreatedAt = DateTime.UtcNow };

            _likeRepoMock.Setup(repo => repo.GetMatchPair(userId1, userId2)).ReturnsAsync(validLikes);
            _messageRepoMock.Setup(repo => repo.GetLastMessageByLikeIdsAsync(It.IsAny<List<string>>())).ReturnsAsync(lastMessage);

            var result = await _sut.GetLastMessageBetweenUsersAsync(userId1, userId2);

            Assert.NotNull(result);
            Assert.Equal(lastMessage.Id, result.Id);
            Assert.Equal(lastMessage.Content, result.Content);
        }

        [Fact]
        public async Task GetConversationAsync_NoMatch_ReturnsEmptyList()
        {
            var userId1 = "user1";
            var userId2 = "user2";
            var incompleteLikes = new List<Like> { new Like { Id = "like1" } };

            _likeRepoMock.Setup(repo => repo.GetMatchPair(userId1, userId2)).ReturnsAsync(incompleteLikes);

            var result = await _sut.GetConversationAsync(userId1, userId2);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetConversationAsync_HasMatch_ReturnsMappedConversation()
        {
            var userId1 = "user1";
            var userId2 = "user2";
            var validLikes = new List<Like> { new Like { Id = "like1" }, new Like { Id = "like2" } };
            var messages = new List<Message> 
            { 
                new Message { Id = "msg1", Content = "Hey", SenderId = userId1, CreatedAt = new DateTime(2023, 1, 1, 12, 0, 0) } 
            };

            _likeRepoMock.Setup(repo => repo.GetMatchPair(userId1, userId2)).ReturnsAsync(validLikes);
            _messageRepoMock.Setup(repo => repo.GetByLikeIdAsync("like1")).ReturnsAsync(messages);

            var result = await _sut.GetConversationAsync(userId1, userId2);

            Assert.Single(result);
            Assert.Equal(messages[0].Id, result[0].Id);
            Assert.Equal(messages[0].Content, result[0].Content);
            Assert.Equal("2023-01-01 12:00", result[0].CreatedAt);
        }
    }
}