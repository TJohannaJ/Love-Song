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
    public class LikeRepository : ILikeRepository
    {
        private readonly MyDbContext _db;

        public LikeRepository(MyDbContext db)
        {
            _db = db;
        }
        public async Task AddOrUpdateAsync(Like like)
        {            
            var existingLike = await _db.Likes.FirstOrDefaultAsync(l => l.Id == like.Id);
            if (existingLike != null) //update existing like
            {
                existingLike.LikedUserId = like.LikedUserId;
                existingLike.UserId = like.UserId;
                existingLike.Mutual = like.Mutual;
                existingLike.CreatedAt = like.CreatedAt;
                _db.Likes.Update(existingLike);
                await _db.SaveChangesAsync();
                return;
            }
            var newlike = new DBLike //add new like to table Likes
            {
                Id = Guid.NewGuid().ToString(),
                LikedUserId = like.LikedUserId,
                UserId = like.UserId,
                Mutual = like.Mutual,
                CreatedAt = like.CreatedAt
            };
            await _db.Likes.AddAsync(newlike);
            await _db.SaveChangesAsync();
            return;
        }

        public async Task<Like> GetByUsers(string userId, string likedUserId)
        {
            var existingLike = await _db.Likes.FirstOrDefaultAsync(l => l.UserId == userId && l.LikedUserId == likedUserId);
            if (existingLike != null) return new Like
            {
                Id = existingLike.Id,
                LikedUserId = existingLike.LikedUserId,
                UserId = existingLike.UserId,
                Mutual = existingLike.Mutual,
                CreatedAt = existingLike.CreatedAt
            };
            return null;
        }

        public async Task<List<string>> GetLikedMutualUsersById(string userId)
        {
            var mutualUserIds = await _db.Likes.Where(l => l.UserId == userId && l.Mutual).Select(l => l.LikedUserId).ToListAsync();
            return mutualUserIds;
        }

        public async Task<List<string>> GetLikedNotMutualUserIdsById(string userId)
        {
            var likedUserIds = await _db.Likes.Where(l => l.UserId == userId && !l.Mutual).Select(l => l.LikedUserId).ToListAsync();
            return likedUserIds;
        }

        public async Task<List<string>> GetLikedUserIdsById(string userId)
        {
            var likedUserIds = await _db.Likes.Where(l => l.UserId == userId).Select(l => l.LikedUserId).ToListAsync();
            return likedUserIds;
        }

        public async Task<List<string>> GetLikedYouUsersById(string userId)
        {
            var likedYouUserIds = await _db.Likes.Where(l => l.LikedUserId == userId && !l.Mutual).Select(l => l.UserId).ToListAsync();
            return likedYouUserIds;
        }

        public async Task<bool> IsMutual(string likeId1, string likeId2)
        {
            var like = await _db.Likes.FirstOrDefaultAsync(l => l.Id == likeId1 || l.Id == likeId2);
            if (like == null) return false;
            return like.Mutual;
        }

        public async Task UndoLike(string userId, string likedUserId)
        {
            //létezik a like
            var existingLike = await _db.Likes.FirstOrDefaultAsync(l => l.UserId == userId && l.LikedUserId == likedUserId);
            if (existingLike == null) return;
            //ha a like mutual, akkor a másik mutual-ja false lesz
            bool isMutual = false;
            if (existingLike.Mutual == true)
            {
                var likedCurrentUser = await _db.Likes.FirstOrDefaultAsync(l => l.UserId == likedUserId && l.LikedUserId == userId);
                if (likedCurrentUser != null)
                {
                    likedCurrentUser.Mutual = isMutual;
                    _db.Likes.Update(likedCurrentUser);
                }
            }
            _db.Likes.Remove(existingLike);
            await _db.SaveChangesAsync();
        }
        public async Task<List<Like>> GetMatchPair(string userId1, string userId2)
        {
            // Lekérjük az entitásokat
            var entities = await _db.Likes
                .Where(l => (l.UserId == userId1 && l.LikedUserId == userId2) ||
                            (l.UserId == userId2 && l.LikedUserId == userId1))
                .ToListAsync();

            // Debug log: Lássuk a konzolon, mit találtunk
            foreach (var e in entities)
            {
                Console.WriteLine($"[DB_CHECK] Like talált: ID={e.Id}, User={e.UserId}, Liked={e.LikedUserId}, Mutual={e.Mutual}");
            }

            return entities.Select(l => new Like
            {
                Id = l.Id,
                UserId = l.UserId,
                LikedUserId = l.LikedUserId,
                Mutual = l.Mutual,
                CreatedAt = l.CreatedAt
            }).ToList();
        }
    }
}
