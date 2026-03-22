using Backend.Domain.Entities;
using Backend.Domain.Interfaces;
using Backend.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Infrastructure.Repositories
{
    internal class UserRepository : IUserRepository
    {
        public List<User> GetRecommendationsForUser(int userId)
        {
            using (var db = new MyDbContext())
            {
                var recommensationsQuery = db.Users.Where(u => u.Id != userId).Take(10).ToList();
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
                }
            }
        }


        public User GetUserById(int userId)
        {
            using (var db = new MyDbContext())
            {
                var userQuery = db.Users.Where(u => u.Id.Equals(userId)).SingleOrDefault();
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
}
