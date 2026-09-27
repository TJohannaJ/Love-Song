using Backend.Domain.Entities;
using Backend.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Domain.Services
{
    public class DiscoveryService
    {
        private readonly IUserRepository _userRepo;
        private readonly ILikeRepository _likeRepo;
        private readonly IDislikeRepository _dislikeRepo;
        private readonly IUserPreferenceRepository _preferenceRepo;

        public DiscoveryService(
            IUserRepository userRepo,
            ILikeRepository likeRepo,
            IDislikeRepository dislikeRepo,
            IUserPreferenceRepository preferenceRepo)
        {
            _userRepo = userRepo;
            _likeRepo = likeRepo;
            _dislikeRepo = dislikeRepo;
            _preferenceRepo = preferenceRepo;
        }

        public async Task<List<User>> GetRecommendationsForUser(string userId)
        {
            var currentUser = await _userRepo.GetUserById(userId);
            if (currentUser == null) return new List<User>();
            var likedUserIds = await _likeRepo.GetLikedUserIdsById(userId);
            var dislikedUserIds = await _dislikeRepo.GetDislikedUserIdsById(userId);
            return await _userRepo.GetRecommendationsForUser(currentUser, likedUserIds, dislikedUserIds);
        }

        public async Task<List<User>> GetLikedYouForUser(string userId)
        {
            var currentUser = await _userRepo.GetUserById(userId);
            if (currentUser == null) return new List<User>();
            var likedYouUserIds = await _likeRepo.GetLikedYouUsersById(userId);
            return await _userRepo.GetLikedYouForUser(currentUser, likedYouUserIds);
        }

        public async Task<List<User>> GetLikesForUser(string userId)
        {
            var currentUser = await _userRepo.GetUserById(userId);
            if (currentUser == null) return new List<User>();
            var likedUserIds = await _likeRepo.GetLikedNotMutualUserIdsById(userId);
            var dislikedUserIds = await _dislikeRepo.GetDislikedUserIdsById(userId);
            likedUserIds = likedUserIds.Except(dislikedUserIds).ToList();
            return await _userRepo.GetLikesForUser(currentUser, likedUserIds);
        }

        public async Task<List<User>> GetDislikesForUser(string userId)
        {
            var currentUser = await _userRepo.GetUserById(userId);
            if (currentUser == null) return new List<User>();
            var dislikedUserIds = await _dislikeRepo.GetDislikedUserIdsById(userId);
            return await _userRepo.GetDislikesForUser(currentUser, dislikedUserIds);
        }

        // --- PREFERENCIA METÓDUSOK ---

        public async Task<UserPreference?> GetUserPreference(string userId)
        {
            return await _preferenceRepo.GetByUserIdAsync(userId);
        }

        public async Task UpdatePreferencePartialAsync(string userId, int? minAge, int? maxAge, string? gender, string? city, bool? commonSongOnly)
        {
            await _preferenceRepo.UpdatePartialAsync(userId, minAge, maxAge, gender, city, commonSongOnly);
        }
    }
}