using Backend.Domain.Entities;
using Backend.Domain.Interfaces;

namespace Backend.Domain.Services
{
    public class RecommendationService
    {
        private IRecommendationRepository _recommendationRepository;
        public RecommendationService(IRecommendationRepository recommendationRepository)
        {
            _recommendationRepository = recommendationRepository;
        }

        public async Task<List<User>> GetRecommendationsForUser(string userId)
        {        
            return await _recommendationRepository.GetRecommendationsForUser(userId);
        }

        public async Task LikeUser(string userId, string likedUserId) 
        { 
            await _recommendationRepository.LikeUser(userId, likedUserId);
        }

        public async Task DislikeUser(string userId, string dislikedUserId)
        { 
            await _recommendationRepository.DislikeUser(userId, dislikedUserId);
        }
    }
}
