using Backend.Domain.Entities;
using Backend.Domain.Interfaces;

namespace Backend.Domain.Services
{
    public class LikesService
    {
        private ILikesRepository _likesRepository;
        public LikesService(ILikesRepository likesRepository)
        {
            _likesRepository = likesRepository;
        }

        public async Task<List<User>> GetLikedYouForUser(string userId)
        {
            return await _likesRepository.GetLikedYouForUser(userId);
        }

        public async Task<List<User>> GetLikesForUser(string userId)
        {
            return await _likesRepository.GetLikesForUser(userId);
        }

        public async Task<List<User>> GetDislikesForUser(string userId)
        {
            return await _likesRepository.GetDislikesForUser(userId);
        }

    }
}
