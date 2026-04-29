using Backend.Domain.DTOs;
using Backend.Domain.Entities;

namespace Backend.Domain.Interfaces
{
    public interface IUserRepository
    {
         Task<User> GetUserById(string userId);
         Task UpdateProfile(string userId, UserDTO model);
        Task<List<User>> GetRecommendationsForUser(User currentUser, List<string> likedUserIds, List<string> dislikedUserIds);
        Task<List<User>> GetLikedYouForUser(User currentUser, List<string> likedYouUserIds);
        Task<List<User>> GetLikesForUser(User currentUser, List<string> likedUsers);
        Task<List<User>> GetDislikesForUser(User currentUser, List<string> dislikedUsers);        
    }
}
