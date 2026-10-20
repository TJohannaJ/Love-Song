using Backend.Domain.DTOs;
using Backend.Domain.Entities;
using Backend.Domain.Interfaces;

namespace Backend.Domain.Services
{
    public class UserService
    {
        private IUserRepository _userRepo;
        private readonly IAiBioGenerator _aiBioGenerator;
        public UserService(IUserRepository userRepo, IAiBioGenerator aiBioGenerator)
        {
            _userRepo = userRepo;
            _aiBioGenerator = aiBioGenerator;
        }

        public async Task<User> GetUser(string userId)
        {
            return await _userRepo.GetUserById(userId);
        }

        public async Task UpdateProfileAsync(string userId, UserDTO model)
        {
            await _userRepo.UpdateProfile(userId, model);
        }
        
        public async Task<GeneratedBioResultDTO> GenerateBioWithAiAsync(string userId, GenerateBioDTO dto, CancellationToken ct = default)
        {
            var user = await _userRepo.GetUserById(userId);
            if (user == null) throw new Exception("User not found");

            return await _aiBioGenerator.GenerateBothBiosAsync(user, dto, ct);
        }
    }
}
