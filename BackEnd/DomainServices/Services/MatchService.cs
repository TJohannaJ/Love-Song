using Backend.Domain.Entities;
using Backend.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Domain.Services
{
    public class MatchService
    {
        private readonly ILikeRepository _likeRepo;
        private readonly IDislikeRepository _dislikeRepo;
        private readonly IUserRepository _userRepo;

        public MatchService(ILikeRepository likeRepo, IDislikeRepository dislikeRepo, IUserRepository userRepo)
        {
            _likeRepo = likeRepo;
            _dislikeRepo = dislikeRepo;
            _userRepo = userRepo;
        }
        public async Task LikeUser(string userId, string likedUserId)
        {
            //user already liked likedUser
            var existingLike = await _likeRepo.GetByUsers(userId, likedUserId);
            if (existingLike != null) return;
            //likedUser liked user previously
            var likedCurrentUser = await _likeRepo.GetByUsers(likedUserId, userId);
            bool isMutual = false;
            //likedUser's like updated if needed
            if (likedCurrentUser != null)
            {
                likedCurrentUser.Mutual = true;
                await _likeRepo.AddOrUpdateAsync(likedCurrentUser);
                isMutual = true;
            }
            //user like
            var like = new Like
            {
                UserId = userId,
                LikedUserId = likedUserId,
                Mutual = isMutual,
                CreatedAt = DateTime.UtcNow
            };
            await _likeRepo.AddOrUpdateAsync(like);
        }
        public async Task DislikeUser(string userId, string dislikedUserId)
        {
            //user already disliked dislikedUser
            var existingDislike = await _dislikeRepo.GetByUsers(userId, dislikedUserId);
            if (existingDislike != null) return;
            //user dislike
            var dislike = new Dislike
            {
                UserId = userId,
                DislikedUserId = dislikedUserId,
                CreatedAt = DateTime.UtcNow
            };
            await _dislikeRepo.AddOrUpdateAsync(dislike);
        }

        public async Task UndoLike(string userId, string likedUserId)
        {
            await _likeRepo.UndoLike(userId, likedUserId);
        }

        public async Task UndoDislike(string userId, string dislikedUserId)
        {
            await _dislikeRepo.UndoDislike(userId, dislikedUserId);
        }

        public async Task<List<User>> GetMatchesForUserAsync(string userId)
        {
            var currentUser = await _userRepo.GetUserById(userId);
            if (currentUser == null) return new List<User>();
            var likedMutualUserIds = await _likeRepo.GetLikedMutualUsersById(userId);
            return await _userRepo.GetMatchesForUser(currentUser, likedMutualUserIds);
        }
    }
}
