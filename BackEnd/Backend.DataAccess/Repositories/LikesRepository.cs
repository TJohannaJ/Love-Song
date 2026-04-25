using Backend.Domain.Entities;
using Backend.Domain.Interfaces;
using Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories
{
    public class LikesRepository : ILikesRepository
    {
        private readonly MyDbContext _db;
        public LikesRepository(MyDbContext db)
        {
            _db = db;
        }

        public async Task<List<User>> GetLikedYouForUser(string userId)
        {
            return await _db.Likes
                .Where(l => l.LikedUserId == userId && !l.Mutual)
                .Join(_db.Users,
                    l => l.UserId,
                    u => u.Id,
                    (l, u) => u)
                .OrderBy(u => Guid.NewGuid()) 
                .Take(20)
                .Select(likedYou => new User
                {
                    Id = likedYou.Id,
                    FullName = likedYou.FullName,
                    BirthDate = likedYou.BirthDate,
                    Age = likedYou.Age,
                    Gender = likedYou.Gender,
                    City = likedYou.City,
                    Preferences = likedYou.Preference,
                    Picture = likedYou.Picture,
                    ShortIntro = likedYou.ShortIntro,
                    LongIntro = likedYou.LongIntro
                }).ToListAsync();          
        }

        public async Task<List<User>> GetLikesForUser(string userId)
        {
            var currentUser = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (currentUser == null) return new List<User>();
            var likedUsers = await _db.Likes.Where(l => l.UserId == userId && l.Mutual == false).Select(l => l.LikedUserId).ToListAsync();
            return await _db.Users.Where(u => likedUsers.Contains(u.Id))
                .OrderBy(u => Guid.NewGuid())
                .Take(20)
                .Select(liked => new User
                {
                    Id = liked.Id,
                    FullName = liked.FullName,
                    BirthDate = liked.BirthDate,
                    Age = liked.Age,
                    Gender = liked.Gender,
                    City = liked.City,
                    Preferences = liked.Preference,
                    Picture = liked.Picture,
                    ShortIntro = liked.ShortIntro,
                    LongIntro = liked.LongIntro
                }).ToListAsync();
        }

        public async Task<List<User>> GetDislikesForUser(string userId)
        {
            var currentUser = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (currentUser == null) return new List<User>();
            var dislikedUsers = await _db.Dislikes.Where(l => l.UserId == userId).Select(l => l.DislikedUserId).ToListAsync();
            return await _db.Users.Where(u => dislikedUsers.Contains(u.Id))
                .OrderBy(u => Guid.NewGuid())
                .Take(20)
                .Select(disliked => new User
                {
                    Id = disliked.Id,
                    FullName = disliked.FullName,
                    BirthDate = disliked.BirthDate,
                    Age = disliked.Age,
                    Gender = disliked.Gender,
                    City = disliked.City,
                    Preferences = disliked.Preference,
                    Picture = disliked.Picture,
                    ShortIntro = disliked.ShortIntro,
                    LongIntro = disliked.LongIntro
                }).ToListAsync();
        }
    }
}
