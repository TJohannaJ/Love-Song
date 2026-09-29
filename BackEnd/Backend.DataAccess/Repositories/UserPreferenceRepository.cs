using Backend.Domain.Entities;
using Backend.Domain.Interfaces;
using Backend.Infrastructure.DBEntities;
using Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories
{
    public class UserPreferenceRepository : IUserPreferenceRepository
    {
        private readonly MyDbContext _db;

        public UserPreferenceRepository(MyDbContext db)
        {
            _db = db;
        }

        public async Task<UserPreference?> GetByUserIdAsync(string userId)
        {
            var dbPref = await _db.UserPreferences.FirstOrDefaultAsync(p => p.UserId == userId);
            if (dbPref == null) return null;

            return new UserPreference(
                dbPref.UserId,
                dbPref.MinAge,
                dbPref.MaxAge,
                dbPref.Gender,
                dbPref.City,
                dbPref.CommonSongOnly
            );
        }

        public async Task UpdatePartialAsync(string userId, int? minAge, int? maxAge, string? gender, string? city, bool? commonSongOnly)
        {
            var existingPref = await _db.UserPreferences.FirstOrDefaultAsync(p => p.UserId == userId);

            if (existingPref == null)
            {
                existingPref = new DBUserPreference
                {
                    UserId = userId,
                    MinAge = minAge ?? 18,
                    MaxAge = maxAge ?? 99,
                    Gender = gender ?? "all",
                    City = string.IsNullOrWhiteSpace(city) ? null : city,
                    CommonSongOnly = commonSongOnly ?? false
                };
                _db.UserPreferences.Add(existingPref);
            }
            else
            {
                if (minAge.HasValue)
                    existingPref.MinAge = minAge.Value;

                if (maxAge.HasValue)
                    existingPref.MaxAge = maxAge.Value;

                if (gender != null)
                    existingPref.Gender = gender;

                // Ha üres string érkezik, állítsuk null-ra az adatbázisban
                if (city != null)
                    existingPref.City = string.IsNullOrWhiteSpace(city) ? null : city;

                if (commonSongOnly.HasValue)
                    existingPref.CommonSongOnly = commonSongOnly.Value;

                _db.UserPreferences.Update(existingPref);
            }

            await _db.SaveChangesAsync();
        }
    }
}