using Backend.Domain.DTOs;
using Backend.Domain.Entities;
using Microsoft.AspNetCore.Identity;
namespace Backend.Domain.Interfaces
{
    public interface IUserRepository
    {
         Task<User> GetUserById(string userId);
         Task UpdateProfile(string userId, UserDTO model, string? uploadedImageUrl = null);
        Task<List<User>> GetRecommendationsForUser(User currentUser, List<string> likedUserIds, List<string> dislikedUserIds);
        Task<List<User>> GetLikedYouForUser(User currentUser, List<string> likedYouUserIds);
        Task<List<User>> GetLikesForUser(User currentUser, List<string> likedUsers);
        Task<List<User>> GetDislikesForUser(User currentUser, List<string> dislikedUsers);
        Task<List<User>> GetMatchesForUser(User currentUser, List<string> likedMutualUserIds);
        Task UpdateMusicVectorAsync(string userId, string musicVector);

        //login register

        Task<User?> FindByEmailAsync(string email);
        Task<User?> FindByRefreshTokenAsync(string refreshToken);
        Task<IdentityResult> CreateAsync(User user, string password, string spotifyToken);
        Task<IdentityResult> UpdateAsync(User user, string? refreshToken = null, DateTime? expiry = null);
        Task<bool> CheckPasswordAsync(User user, string password);
        Task<IdentityResult> SetRefreshTokenAsync(User user, string refreshToken, DateTime dateTime);
    }
}
