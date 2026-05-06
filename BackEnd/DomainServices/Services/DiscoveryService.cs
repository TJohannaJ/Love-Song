using Backend.Domain.Entities;
using Backend.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Domain.Services
{
    public class DiscoveryService
    {
        private readonly IUserRepository _userRepo;
        private readonly ILikeRepository _likeRepo;
        private readonly IDislikeRepository _dislikeRepo;

        public DiscoveryService(IUserRepository userRepo, ILikeRepository likeRepo, IDislikeRepository dislikeRepo)
        {
            _userRepo = userRepo;
            _likeRepo = likeRepo;
            _dislikeRepo = dislikeRepo;
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
            return await _userRepo.GetLikesForUser(currentUser, likedUserIds);
        }

        public async Task<List<User>> GetDislikesForUser(string userId)
        {
            var currentUser = await _userRepo.GetUserById(userId);
            if (currentUser == null) return new List<User>();
            var dislikedUserIds = await _dislikeRepo.GetDislikedUserIdsById(userId);
            return await _userRepo.GetDislikesForUser(currentUser, dislikedUserIds);
        }

    }
}
