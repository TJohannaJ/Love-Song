using Backend.Domain.Entities;
using Backend.Domain.Interfaces;
using Backend.Infrastructure.DBEntities;
using Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly MyDbContext _context;

        public MessageRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task<Message> GetByIdAsync(string id)
        {
            var dbMessage = await _context.Messages.FindAsync(id);
            if (dbMessage == null) return null;
            return new Message
            {
                Id = dbMessage.Id,
                Content = dbMessage.Content,
                SenderId = dbMessage.SenderId,
                ReceiverId = dbMessage.ReceiverId,
                LikeId1 = dbMessage.LikeId1,
                LikeId2 = dbMessage.LikeId2,
                CreatedAt = dbMessage.CreatedAt,
                Seen = dbMessage.Seen
            };
        }
        public async Task<IEnumerable<Message>> GetByLikeIdAsync(string likeId)
        {
            // Olyan üzeneteket keresünk, ahol a megadott ID vagy az első, vagy a második LikeId helyen szerepel
            return await _context.Messages
                .Where(m => m.LikeId1 == likeId || m.LikeId2 == likeId)
                .OrderBy(m => m.CreatedAt) // Fontos az időrend!
                .Select(m => new Message
                {
                    Id = m.Id,
                    Content = m.Content,
                    SenderId = m.SenderId,
                    ReceiverId = m.ReceiverId,
                    CreatedAt = m.CreatedAt
                })
                .ToListAsync();
        }

        public async Task AddAsync(Message message)
        {
            var dbMessage = new DBMessage
            {
                Id = Guid.NewGuid().ToString(),
                Content = message.Content,
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                LikeId1 = message.LikeId1,
                LikeId2 = message.LikeId2,
                CreatedAt = message.CreatedAt,
                Seen = message.Seen
            };
            await _context.Messages.AddAsync(dbMessage);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<Message?> GetLastMessageByLikeIdsAsync(List<string> likeIds)
        {
            var m = await _context.Messages
                .Where(m => likeIds.Contains(m.LikeId1) || likeIds.Contains(m.LikeId2))
                .OrderByDescending(m => m.CreatedAt)
                .FirstOrDefaultAsync();

            if (m == null) return null;

            return new Message
            {
                Id = m.Id,
                Content = m.Content,
                SenderId = m.SenderId,
                ReceiverId = m.ReceiverId,
                CreatedAt = m.CreatedAt
            };
        }
    }
}