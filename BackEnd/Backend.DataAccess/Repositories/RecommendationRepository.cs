using Backend.Domain.Entities;
using Backend.Domain.Interfaces;
using Backend.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Infrastructure.Repositories
{
    public class RecommendationRepository : IRecommendationRepository
    {
        private readonly MyDbContext _db;
        public RecommendationRepository(MyDbContext db)
        {
            _db = db;
        }
        public List<User> GetRecommendationsForUser(string userId)
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
                        FullName = recommended.FullName,
                        BirthDate = recommended.BirthDate,
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

    }
}
