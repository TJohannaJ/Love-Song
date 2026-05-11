using Backend.Domain.DTOs;
using Backend.Domain.Entities;
using Backend.Domain.Interfaces;
using Backend.Infrastructure.DBEntities;
using Backend.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MyDbContext _db;
        private readonly UserManager<DBUser> _userManager;
        public UserRepository(MyDbContext db, UserManager<DBUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<List<User>> GetDislikesForUser(User currentUser, List<string> dislikedUsers)
        {
            return await _db.Users.Where(u => dislikedUsers.Contains(u.Id)).Take(20).Select(disliked => new User
            {
                Id = disliked.Id,
                FullName = disliked.FullName,
                BirthDate = disliked.BirthDate,
                Age = disliked.Age,
                Gender = disliked.Gender,
                City = disliked.City,
                Preference = disliked.Preference,
                Picture = disliked.Picture,
                ShortIntro = disliked.ShortIntro,
                LongIntro = disliked.LongIntro
            }).ToListAsync();
        }

        public async Task<List<User>> GetLikedYouForUser(User currentUser, List<string> likedYouUserIds)
        {
            return await _db.Users.Where(u => likedYouUserIds.Contains(u.Id)).Take(20).Select(liked => new User
            {
                Id = liked.Id,
                FullName = liked.FullName,
                BirthDate = liked.BirthDate,
                Age = liked.Age,
                Gender = liked.Gender,
                City = liked.City,
                Preference = liked.Preference,
                Picture = liked.Picture,
                ShortIntro = liked.ShortIntro,
                LongIntro = liked.LongIntro
            }).ToListAsync();
        }

        public async Task<List<User>> GetLikesForUser(User currentUser, List<string> likedUsers)
        {
            return await _db.Users.Where(u => likedUsers.Contains(u.Id)).Take(20).Select(liked => new User
            {
                Id = liked.Id,
                FullName = liked.FullName,
                BirthDate = liked.BirthDate,
                Age = liked.Age,
                Gender = liked.Gender,
                City = liked.City,
                Preference = liked.Preference,
                Picture = liked.Picture,
                ShortIntro = liked.ShortIntro,
                LongIntro = liked.LongIntro
            }).ToListAsync();
        }

        public async Task<List<User>> GetMatchesForUser(User currentUser, List<string> likedMutualUserIds)
        {
            return await _db.Users.Where(u => likedMutualUserIds.Contains(u.Id)).Take(20).Select(liked => new User
            {
                Id = liked.Id,
                FullName = liked.FullName,
                BirthDate = liked.BirthDate,
                Age = liked.Age,
                Gender = liked.Gender,
                City = liked.City,
                Preference = liked.Preference,
                Picture = liked.Picture,
                ShortIntro = liked.ShortIntro,
                LongIntro = liked.LongIntro
            }).ToListAsync();
        }

        public async Task<List<User>> GetRecommendationsForUser(User currentUser, List<string> likedUserIds, List<string> dislikedUserIds)
        {
            var query = _db.Users.Where(u => u.Id != currentUser.Id && !likedUserIds.Contains(u.Id) && !dislikedUserIds.Contains(u.Id));
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
                    Preference = recommended.Preference,
                    Picture = recommended.Picture,
                    ShortIntro = recommended.ShortIntro,
                    LongIntro = recommended.LongIntro
                })
                .ToListAsync();
        }

        public async Task<User> GetUserById(string userId) 
        {
            var currentUser = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (currentUser == null) return null;
            return new User { 
                Id = currentUser.Id,
                City = currentUser.City,
                FullName = currentUser.FullName,
                BirthDate = currentUser.BirthDate,
                Gender = currentUser.Gender, 
                ShortIntro = currentUser.ShortIntro,
                LongIntro = currentUser.LongIntro,
                Picture = currentUser.Picture,
                Age = currentUser.Age,
                Preference = currentUser.Preference
            };
        }

        public async Task UpdateProfile(string userId, UserDTO model)
        {
            var user =await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) throw new Exception("User not found");
            if(!string.IsNullOrEmpty(model.FullName))
                user.FullName = model.FullName;
            if (model.Picture != null && model.Picture.Length > 0)
                user.Picture = model.Picture;
            if(!string.IsNullOrEmpty(model.City))
                user.City = model.City;
            if(!string.IsNullOrEmpty(model.Gender))
                user.Gender = model.Gender;
            if(!string.IsNullOrEmpty(model.ShortIntro))
                user.ShortIntro = model.ShortIntro;
            if(!string.IsNullOrEmpty(model.LongIntro))
                user.LongIntro = model.LongIntro;
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
        }

        //login register

        public async Task<User?> FindByEmailAsync(string email)
        {
            var dbUser = await _userManager.FindByEmailAsync(email);
            return MapToDomain(dbUser);
        }

        public async Task<User?> FindByRefreshTokenAsync(string refreshToken)
        {            
            var dbUser = await _db.Users
                .FirstOrDefaultAsync(u => _db.UserTokens
                    .Any(t => t.UserId == u.Id && t.Name == "RefreshToken" && t.Value == refreshToken));

            return MapToDomain(dbUser);
        }

        public async Task<IdentityResult> CreateAsync(User user, string password, string spotifyToken)
        {
            var dbUser = new DBUser
            {
                UserName = user.Email,
                Email = user.Email,
                FullName = user.FullName,
                BirthDate = user.BirthDate,
                City = user.City,
                Gender = user.Gender,
                Preference = user.Preference,
                Picture = user.Picture,
                ShortIntro = user.ShortIntro,
                LongIntro = user.LongIntro,
                SpotifyToken = spotifyToken
            };
            return await _userManager.CreateAsync(dbUser, password);
        }

        public async Task<IdentityResult> SetRefreshTokenAsync(User user, string refreshToken, DateTime expiry)
        {
            var dbUser = await _userManager.FindByIdAsync(user.Id);

            if (dbUser == null)
            {
                throw new Exception($"FK HIBA ELŐZÉS: A user {user.Id} nem található a DB-ben!");
            }

            return await _userManager.SetAuthenticationTokenAsync(dbUser, "LocalApp", "RefreshToken", refreshToken);
        }

        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            var dbUser = await _userManager.FindByIdAsync(user.Id);
            return dbUser != null && await _userManager.CheckPasswordAsync(dbUser, password);
        }

        public async Task<IdentityResult> UpdateAsync(User user, string? refreshToken = null, DateTime? expiry = null)
        {
            var dbUser = await _userManager.FindByIdAsync(user.Id);
            if (dbUser == null) return IdentityResult.Failed();

            dbUser.FullName = user.FullName;
            dbUser.City = user.City;
            dbUser.Gender = user.Gender;
            dbUser.Preference = user.Preference;
            dbUser.ShortIntro = user.ShortIntro;
            dbUser.LongIntro = user.LongIntro;
            dbUser.Picture = user.Picture;

            return await _userManager.UpdateAsync(dbUser);
        }

        private User? MapToDomain(DBUser? dbUser)
        {
            if (dbUser == null) return null;
            return new User(
                dbUser.Id,
                dbUser.FullName,
                dbUser.Age,
                dbUser.Gender,
                dbUser.City,
                dbUser.Preference,
                dbUser.Picture,
                dbUser.ShortIntro,
                dbUser.LongIntro)
            {
                Email = dbUser.Email,
                BirthDate = dbUser.BirthDate
            };
        }
    }
}
