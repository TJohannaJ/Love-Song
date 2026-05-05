using Backend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Domain.Interfaces
{
    public interface ILikeRepository
    {
        Task<Like> GetByUsers(string userId, string likedUserId);
        Task AddOrUpdateAsync(Like like);
        Task UndoLike(string userId, string likedUserId);
        Task<bool> IsMutual(string likeId1, string likeId2);
        Task<List<string>> GetLikedUserIdsById(string userId);
        Task<List<string>> GetLikedNotMutualUserIdsById(string userId);
        Task<List<string>> GetLikedYouUsersById(string userId);
        Task<List<string>> GetLikedMutualUsersById(string userId);
        Task<List<Like>> GetMatchPair(string user1, string user2);
    }
}
