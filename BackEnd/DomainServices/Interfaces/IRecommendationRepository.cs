using Backend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Domain.Interfaces
{
    public interface IRecommendationRepository
    {     
        List<User> GetRecommendationsForUser(string userId);
        void LikeUser(string currentUserId, string likedUserId);
        void DislikeUser(string currentUserId, string likedUserId);
    }
}
