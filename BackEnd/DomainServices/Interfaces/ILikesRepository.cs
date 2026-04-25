using Backend.Domain.Entities;

namespace Backend.Domain.Interfaces
{
    public interface ILikesRepository
    {
        Task<List<User>> GetLikedYouForUser(string userId);
        Task<List<User>> GetLikesForUser(string userId);
        Task<List<User>> GetDislikesForUser(string userId);
    }
}
