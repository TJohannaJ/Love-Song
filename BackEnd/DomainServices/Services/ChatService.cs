// Application / Services / ChatService.cs
using Backend.Domain.DTOs;
using Backend.Domain.Entities;
using Backend.Domain.Interfaces;

namespace Backend.Domain.Services
{
    public class ChatService
    {
        private readonly IMessageRepository _messageRepo;
        private readonly ILikeRepository _likeRepo;

        public ChatService(IMessageRepository messageRepo, ILikeRepository likeRepo)
        {
            _messageRepo = messageRepo;
            _likeRepo = likeRepo;
        }

        public async Task<MessageResponseDTO> SaveMessageAsync(SendMessageRequestDTO request)
        {
            var isMutual = await _likeRepo.IsMutual(request.LikeId);
            if (!isMutual)
                throw new UnauthorizedAccessException("Nincs érvényes match a felek között.");

            var message = new Message
            {
                LikeId = request.LikeId,
                SenderId = request.SenderId,
                ReceiverId = request.ReceiverId,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow
            };

            await _messageRepo.AddAsync(message);

            return new MessageResponseDTO
            {
                Id = message.Id,
                Content = message.Content,
                SenderId = message.SenderId,
                CreatedAt = message.CreatedAt
            };
        }

        public async Task<IEnumerable<MessageResponseDTO>> GetChatHistoryAsync(string likeId)
        {
            var messages = await _messageRepo.GetByLikeIdAsync(likeId);

            return messages.Select(m => new MessageResponseDTO
            {
                Id = m.Id,
                Content = m.Content,
                SenderId = m.SenderId,
                CreatedAt = m.CreatedAt
            });
        }

    }
}

