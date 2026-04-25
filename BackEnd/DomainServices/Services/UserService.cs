using Backend.Domain.DTOs;
using Backend.Domain.Entities;
using Backend.Domain.Interfaces;

namespace Backend.Domain.Services
{
    public class UserService
    {
        private IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> GetUser(string userId)
        {
            return await _userRepository.GetUser(userId);
        }

        public async Task UpdateProfileAsync(string userId, UserDTO model)
        {
            await _userRepository.UpdateProfile(userId, model);
        }
    }
}
