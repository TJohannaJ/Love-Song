using Backend.Domain.DTOs;
using Backend.Domain.Entities;

namespace Backend.Domain.Interfaces
{
    public interface IUserRepository
    {
         Task<User> GetUser(string userId);
         Task UpdateProfile(string userId, UserDTO model);
    }
}
