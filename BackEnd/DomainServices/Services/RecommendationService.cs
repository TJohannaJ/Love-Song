using Backend.Domain.Entities;
using Backend.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Backend.Domain.Services
{
    public class RecommendationService
    {
        private IRecommendationRepository _recommendationRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RecommendationService(IRecommendationRepository recommendationRepository, IHttpContextAccessor httpContextAccessor)
        {
            _recommendationRepository = recommendationRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public List<User> GetRecommendationsForUser()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException("Nincs bejelentkezett felhasználó vagy érvénytelen token.");
            }

            var currentUserId = userIdClaim.Value;

            return _recommendationRepository.GetRecommendationsForUser(currentUserId);
        }

        public void LikeUser(string likedUserId)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException("Nincs bejelentkezett felhasználó vagy érvénytelen token.");
            }

            var currentUserId = userIdClaim.Value;

            _recommendationRepository.LikeUser(currentUserId, likedUserId);
        }

        public void DislikeUser(string dislikedUserId)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException("Nincs bejelentkezett felhasználó vagy érvénytelen token.");
            }

            var currentUserId = userIdClaim.Value;

            _recommendationRepository.DislikeUser(currentUserId, dislikedUserId);
        }
    }
}
