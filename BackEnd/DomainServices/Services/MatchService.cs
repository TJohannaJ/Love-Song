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
        private readonly IEmailService _emailService;

        public MatchService(ILikeRepository likeRepo, IDislikeRepository dislikeRepo, IUserRepository userRepo, IEmailService emailService) 
        {
            _likeRepo = likeRepo;
            _dislikeRepo = dislikeRepo;
            _userRepo = userRepo;
            _emailService = emailService;
        }
        public async Task LikeUser(string userId, string likedUserId)
        {
            // A like és a dislike kölcsönösen kizárják egymást.
            await _dislikeRepo.UndoDislike(userId, likedUserId); //

            // user already liked likedUser
            var existingLike = await _likeRepo.GetByUsers(userId, likedUserId); //
            if (existingLike != null) return; //[cite: 6]

            // likedUser liked user previously
            var likedCurrentUser = await _likeRepo.GetByUsers(likedUserId, userId); //[cite: 6]
            bool isMutual = false; //[cite: 6]

            // likedUser's like updated if needed
            if (likedCurrentUser != null) //[cite: 6]
            {
                likedCurrentUser.Mutual = true; //[cite: 6]
                await _likeRepo.AddOrUpdateAsync(likedCurrentUser); //[cite: 6]
                isMutual = true; //[cite: 6]
            }

            // user like
            var like = new Like
            {
                UserId = userId,
                LikedUserId = likedUserId,
                Mutual = isMutual,
                CreatedAt = DateTime.UtcNow
            }; //[cite: 6]
            await _likeRepo.AddOrUpdateAsync(like); //[cite: 6]

            // =========================================================
            // 3. EMAILEK KÜLDÉSE - Kizárólag akkor, ha MUTUAL (Match) van!
            // =========================================================
            if (isMutual)
            {
                var currentUser = await _userRepo.GetUserById(userId);
                var matchedUser = await _userRepo.GetUserById(likedUserId);

                if (currentUser != null && matchedUser != null)
                {
                    // A Task.Run segítségével a háttérben küldjük el az emaileket,
                    // így a felhasználó kérése nem akad meg és gyors válasz érkezik a Frontendnek.

                    // 1. Értesítés a most like-oló felhasználónak
                    _ = Task.Run(async () => await _emailService.SendMatchNotificationAsync(
                        currentUser.Email, 
                        currentUser.FullName, 
                        matchedUser.FullName));

                    // 2. Értesítés a korábban like-oló másik felhasználónak
                    _ = Task.Run(async () => await _emailService.SendMatchNotificationAsync(
                        matchedUser.Email, 
                        matchedUser.FullName, 
                        currentUser.FullName));
                }
            }
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

            // A like és a dislike kölcsönösen kizárják egymást.
            await _likeRepo.UndoLike(userId, dislikedUserId);
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
