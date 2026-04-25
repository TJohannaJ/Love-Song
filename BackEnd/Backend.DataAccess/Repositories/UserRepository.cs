using Backend.Domain.Entities;
using Backend.Domain.Interfaces;
using Backend.Infrastructure.Persistence;
using Backend.Domain.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MyDbContext _db;       
        public UserRepository(MyDbContext db)
        {
            _db = db;
        }

        public async Task<User> GetUser(string userId) 
        {
            var currentUser = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (currentUser == null) return null;
            return new User { 
                Id = currentUser.Id,
                City = currentUser.City,
                FullName = currentUser.FullName,
                BirthDate = currentUser.BirthDate,
                Gender = currentUser.Gender, 
                ShortIntro=currentUser.ShortIntro,
                LongIntro=currentUser.LongIntro,
                Picture=currentUser.Picture 
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
    }
}
