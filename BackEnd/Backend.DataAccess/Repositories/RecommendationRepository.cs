using Backend.Domain.Entities;
using Backend.Domain.Interfaces;
using Backend.Infrastructure.DBEntities;
using Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories
{
    public class RecommendationRepository : IRecommendationRepository
    {
        private readonly MyDbContext _db;
        public RecommendationRepository(MyDbContext db)
        {
            _db = db;
        }
        public async Task<List<User>> GetRecommendationsForUser(string userId)
        {
            var currentUser = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (currentUser == null) return new List<User>();
            var likedUserIds = await _db.Likes.Where(l => l.UserId == userId).Select(l => l.LikedUserId).ToListAsync();
            var dislikedUserIds = await _db.Dislikes.Where(d => d.UserId == userId).Select(d => d.DislikedUserId).ToListAsync();
            var query = _db.Users.Where(u => u.Id != userId && !likedUserIds.Contains(u.Id) && !dislikedUserIds.Contains(u.Id));
            if (currentUser.Preference == "male" || currentUser.Preference == "female")
            {
                query = query.Where(u => u.Gender == currentUser.Preference);
            }
            return await query
                .OrderBy(u => Guid.NewGuid()) //randomizálja a recommended userek sorrendjét
                .Take(20)
                .Select(recommended => new User
                {
                    Id = recommended.Id,
                    FullName = recommended.FullName,
                    BirthDate = recommended.BirthDate,
                    Age = recommended.Age,
                    Gender = recommended.Gender,
                    City = recommended.City,
                    Preferences = recommended.Preference,
                    Picture = recommended.Picture,
                    ShortIntro = recommended.ShortIntro,
                    LongIntro = recommended.LongIntro
                })
                .ToListAsync();
        }

        public async Task LikeUser(string userId, string likedUserId)
        {
            //már likeolta korábban
            var existingLike = await _db.Likes.FirstOrDefaultAsync(l => l.UserId == userId && l.LikedUserId == likedUserId);
            if(existingLike != null) return;
            //már a másik irányba likeolva volt
            var likedCurrentUser =await _db.Likes.FirstOrDefaultAsync(l => l.UserId == likedUserId && l.LikedUserId == userId);
            bool isMutual = false;
            if(likedCurrentUser != null)
            {
                likedCurrentUser.Mutual = true;
                _db.Likes.Update(likedCurrentUser);
                isMutual = true;
            }
            //most kell egy új like
            var like = new DBLike
            {
                UserId = userId,
                LikedUserId = likedUserId,
                CreatedAt = DateTime.UtcNow,
                Mutual = isMutual
            };
            await _db.Likes.AddAsync(like);
            await _db.SaveChangesAsync();

        }

        public async Task DislikeUser(string userId, string dislikedUserId)
        {
            //már dislikeolta korábban
            var existingDisike =await _db.Dislikes.FirstOrDefaultAsync(l => l.UserId == userId && l.DislikedUserId == dislikedUserId);
            if (existingDisike != null) return;
            
            //most kell egy új dislike
            var dislike = new DBDislike
            {
                UserId = userId,
                DislikedUserId = dislikedUserId,
                CreatedAt = DateTime.UtcNow
            };
            await _db.Dislikes.AddAsync(dislike);
            await _db.SaveChangesAsync();

        }
    }
}
