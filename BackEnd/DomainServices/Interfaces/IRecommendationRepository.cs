using Backend.Domain.Entities;

namespace Backend.Domain.Interfaces
{
    public interface IRecommendationRepository
    {     
        Task<List<User>> GetRecommendationsForUser(string userId);
        Task LikeUser(string currentUserId, string likedUserId);
        Task DislikeUser(string currentUserId, string likedUserId);
    }
}
