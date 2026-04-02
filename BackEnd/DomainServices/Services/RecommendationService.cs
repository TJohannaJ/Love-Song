using Backend.Domain.Entities;
using Backend.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Domain.Services
{
    public class RecommendationService
    {
        private IRecommendationRepository recommendationRepository;
        public RecommendationService(IRecommendationRepository recommendationRepository)
        {
            this.recommendationRepository = recommendationRepository;
        }
        public List<User> GetRecommendationsForUser(string userId)
        {
            return recommendationRepository.GetRecommendationsForUser(userId);
        }
    }
}
