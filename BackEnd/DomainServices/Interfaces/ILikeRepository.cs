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
        Task<bool> IsMutual(string likeId);
        Task<List<string>> GetLikedUserIdsById(string userId);
        Task<List<string>> GetLikedNotMutualUserIdsById(string userId);
        Task<List<string>> GetLikedYouUsersById(string userId);
    }
}
