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
                LikeId = dbMessage.LikeId,
                CreatedAt = dbMessage.CreatedAt,
                Seen = dbMessage.Seen
            };
        }
        public async Task<IEnumerable<Message>> GetByLikeIdAsync(string likeId)
        {
            // Lekérjük az összes üzenetet egy adott match-hez, időrendben
            return await _context.Messages
                .Where(m => m.LikeId == likeId)
                .OrderBy(m => m.CreatedAt)
                .Select(m => new Message
                {
                    Id = m.Id,
                    Content = m.Content,
                    SenderId = m.SenderId,
                    ReceiverId = m.ReceiverId,
                    LikeId = m.LikeId,
                    CreatedAt = m.CreatedAt,
                    Seen = m.Seen
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
                LikeId = message.LikeId,
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
    }
}