using Backend.Domain.DTOs;
using Backend.Domain.Entities;
using Backend.Domain.Interfaces;

namespace Backend.Domain.Services
{
    public class UserService
    {
        private IUserRepository _userRepo;
        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<User> GetUser(string userId)
        {
            return await _userRepo.GetUserById(userId);
        }

        public async Task UpdateProfileAsync(string userId, UserDTO model)
        {
            await _userRepo.UpdateProfile(userId, model);
        }
    }
}
