using Backend.Domain.Entities;
using Backend.Domain.Interfaces;
using Backend.Infrastructure.DBEntities;
using Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Infrastructure.Repositories
{
    public class DislikeRepository : IDislikeRepository
    {
        private readonly MyDbContext _db;

        public DislikeRepository(MyDbContext db)
        {
            _db = db;
        }

        public async Task AddOrUpdateAsync(Dislike dislike)
        {
            var existingDislike = await _db.Dislikes.FirstOrDefaultAsync(d => d.Id == dislike.Id);
            if (existingDislike != null) //update existing like
            {
                existingDislike.DislikedUserId = dislike.DislikedUserId;
                existingDislike.UserId = dislike.UserId;
                existingDislike.CreatedAt = dislike.CreatedAt;
                _db.Dislikes.Update(existingDislike);
                await _db.SaveChangesAsync();
                return;
            }
            var newDislike = new DBDislike //add new dislike to table Dislikes
            {
                Id = Guid.NewGuid().ToString(),
                DislikedUserId = dislike.DislikedUserId,
                UserId = dislike.UserId,
                CreatedAt = dislike.CreatedAt
            };
            await _db.Dislikes.AddAsync(newDislike);
            await _db.SaveChangesAsync();
            return;
        }

        public async Task<Dislike> GetByUsers(string userId, string dislikedUserId)
        {
            var existingDislike = await _db.Dislikes.FirstOrDefaultAsync(d => d.UserId == userId && d.DislikedUserId == dislikedUserId);
            if (existingDislike != null) return new Dislike
            {
                Id = existingDislike.Id,
                DislikedUserId = existingDislike.DislikedUserId,
                UserId = existingDislike.UserId,
                CreatedAt = existingDislike.CreatedAt
            };
            return null;
        }

        public async Task<List<string>> GetDislikedUserIdsById(string userId)
        {
            var dislikedUserIds = await _db.Dislikes.Where(d => d.UserId == userId).Select(d => d.DislikedUserId).ToListAsync();
            return dislikedUserIds;
        }

        public async Task UndoDislike(string userId, string dislikedUserId)
        {
            //már dislikeolta korábban
            var existingDislike = _db.Dislikes.FirstOrDefault(d => d.UserId == userId && d.DislikedUserId == dislikedUserId);
            if (existingDislike != null)
            {
                //most kell egy dislike törlése 
                _db.Dislikes.Remove(existingDislike);
                await _db.SaveChangesAsync();
            }
        }
    }
}
