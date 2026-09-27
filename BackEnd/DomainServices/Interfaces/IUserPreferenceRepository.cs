using Backend.Domain.DTOs;
using Backend.Domain.Entities;

namespace Backend.Domain.Interfaces
{
    public interface IUserPreferenceRepository
    {
        Task<UserPreference?> GetByUserIdAsync(string userId);
        Task UpdatePartialAsync(string userId, int? minAge, int? maxAge, string? gender, string? city, bool? commonSongOnly);
    }
}
