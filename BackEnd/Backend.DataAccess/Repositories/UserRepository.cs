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

        // context injected by DI
        public UserRepository(MyDbContext db)
        {
            _db = db;
        }
        public List<User> GetRecommendationsForUser(int userId)
        {
            var recommensationsQuery = _db.Users.Where(u => u.Id != userId).Take(10).ToList();
            if (recommensationsQuery != null)
            {
                var recommendations = new List<User>();
                foreach (var recommended in recommensationsQuery)
                {
                    recommendations.Add(new User
                    {
                        Id = recommended.Id,
                        FullName = recommended.Name,
                        Age = recommended.Age,
                        Gender = recommended.Gender,
                        City = recommended.City,
                        Preferences = recommended.Preference,
                        Picture = recommended.Picture,
                        ShortIntro = recommended.ShortIntro,
                        LongIntro = recommended.LongIntro
                    });
                }
                return recommendations;
            }
            else
            {
                return new List<User>();
            }
        }


        public User GetUserById(int userId)
        {
            var userQuery = _db.Users.Where(u => u.Id.Equals(userId)).SingleOrDefault();
            if (userQuery != null)
            {
                return new User
                {
                    Id = userId,
                    FullName = userQuery.Name,
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
