using Backend.Domain.Entities;
using Backend.Domain.Interfaces;
using Backend.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Backend.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MyDbContext _db;       
        public UserRepository(MyDbContext db)
        {
            _db = db;
        }     

        public User GetUserById(string userId)
        {
            var userQuery = _db.Users.Where(u => u.Id.Equals(userId)).SingleOrDefault();
            if (userQuery != null)
            {
                return new User
                {
                    Id = userId,
                    FullName = userQuery.FullName,
                    Age = userQuery.Age,
                    Gender = userQuery.Gender,
                    City = userQuery.City,
                    Preferences = userQuery.Preference,
                    Picture = userQuery.Picture,
                    ShortIntro = userQuery.ShortIntro,
                    LongIntro = userQuery.LongIntro
                };
            }
            else
            {
                return null;
            }
        }
    }
}
