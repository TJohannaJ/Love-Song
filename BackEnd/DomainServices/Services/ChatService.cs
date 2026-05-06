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
            var likes = await _likeRepo.GetMatchPair(request.SenderId, request.ReceiverId);

            // Ellenőrizzük, hogy valóban megvan-e a kettő és nem üresek az ID-k
            var likeAtoB = likes.FirstOrDefault(l => l.UserId != null && l.UserId.Equals(request.SenderId, StringComparison.OrdinalIgnoreCase));
            var likeBtoA = likes.FirstOrDefault(l => l.UserId != null && l.UserId.Equals(request.ReceiverId, StringComparison.OrdinalIgnoreCase));

            if (likeAtoB?.Id == null || likeBtoA?.Id == null)
            {
                throw new Exception("Hiba: Nem találhatók érvényes Like ID-k a mentéshez.");
            }

            var message = new Message
            {
                Id = Guid.NewGuid().ToString(),
                LikeId1 = likeAtoB.Id,
                LikeId2 = likeBtoA.Id,
                SenderId = request.SenderId,
                ReceiverId = request.ReceiverId,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow,
                Seen = false
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
        public async Task<MessageResponseDTO?> GetLastMessageBetweenUsersAsync(string userId1, string userId2)
        {
            // Lekérjük a match-hez tartozó like-okat
            var likes = await _likeRepo.GetMatchPair(userId1, userId2);
            if (likes.Count < 2) return null;

            var likeIds = likes.Select(l => l.Id).ToList();

            // A repository-n keresztül lekérjük az utolsó üzenetet
            var lastMessage = await _messageRepo.GetLastMessageByLikeIdsAsync(likeIds);

            if (lastMessage == null) return null;

            return new MessageResponseDTO
            {
                Id = lastMessage.Id,
                Content = lastMessage.Content,
                SenderId = lastMessage.SenderId,
                CreatedAt = lastMessage.CreatedAt
            };
        }
        public async Task<List<MessageDTO>> GetConversationAsync(string userId, string otherUserId)
        {
            // Megkeressük a két felhasználó közötti közös like-okat (a match-et)
            var likes = await _likeRepo.GetMatchPair(userId, otherUserId);
            if (likes.Count < 2) return new List<MessageDTO>();

            // Elég az egyik LikeId alapján keresni, mert a repository mindkét irányt nézi 
            // (vagy a korábban javasolt List<string> alapú keresést használjuk)
            var likeId = likes.First().Id;
            var messages = await _messageRepo.GetByLikeIdAsync(likeId);

            return messages.Select(m => new MessageDTO
            {
                Id = m.Id,
                Content = m.Content,
                SenderId = m.SenderId,
                CreatedAt = m.CreatedAt.ToString("yyyy-MM-dd HH:mm")
            }).ToList();
        }
    }
}

