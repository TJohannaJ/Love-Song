using Backend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Domain.Interfaces
{
    public interface IDislikeRepository
    {
        Task<Dislike> GetByUsers(string userId, string dislikedUserId);
        Task AddOrUpdateAsync(Dislike dislike);
        Task<List<string>> GetDislikedUserIdsById(string userId);
        Task UndoDislike(string userId, string dislikedUserId);
    }
}
