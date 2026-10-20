using Moq;
using Xunit;
using Backend.Domain.Services;
using Backend.Domain.Interfaces;
using Backend.Domain.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Backend.Domain.Tests.Services
{
	public class AiChatServiceTests
	{
		private readonly Mock<IAiChatBot> _aiChatBotMock;
		private readonly ChatBotService _sut;

		public AiChatServiceTests()
		{
			_aiChatBotMock = new Mock<IAiChatBot>();
			_sut = new ChatBotService(_aiChatBotMock.Object);
		}

		[Fact]
		public async Task ProcessMessageAsync_CallsAiChatBotAndReturnsResult()
		{
			var history = new List<ChatMessageDTO>
			{
				new ChatMessageDTO { Role = "user", Content = "Hello" }
			};
			var cancellationToken = new CancellationToken();
			var expectedResponse = new AiChatResponseDTO { Reply = "Hi there!" };

			_aiChatBotMock.Setup(bot => bot.AskAsync(history, cancellationToken))
				.ReturnsAsync(expectedResponse);

			var result = await _sut.ProcessMessageAsync(history, cancellationToken);

			Assert.Equal(expectedResponse, result);
			_aiChatBotMock.Verify(bot => bot.AskAsync(history, cancellationToken), Times.Once);
		}
	}
}