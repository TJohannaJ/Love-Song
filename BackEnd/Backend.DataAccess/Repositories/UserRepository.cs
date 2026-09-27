using Backend.Domain.DTOs;
using Backend.Domain.Entities;
using Backend.Domain.Interfaces;
using Backend.Infrastructure.DBEntities;
using Backend.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MyDbContext _db;
        private readonly UserManager<DBUser> _userManager;

        public UserRepository(MyDbContext db, UserManager<DBUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<List<User>> GetDislikesForUser(User currentUser, List<string> dislikedUsers)
        {
            var dbUsers = await _db.Users
                .Where(u => dislikedUsers.Contains(u.Id))
                .Take(20)
                .ToListAsync();

            return dbUsers.Select(MapToDomain).Where(u => u != null).ToList()!;
        }

        public async Task<List<User>> GetLikedYouForUser(User currentUser, List<string> likedYouUserIds)
        {
            var dbUsers = await _db.Users
                .Where(u => likedYouUserIds.Contains(u.Id))
                .Take(20)
                .ToListAsync();

            return dbUsers.Select(MapToDomain).Where(u => u != null).ToList()!;
        }

        public async Task<List<User>> GetLikesForUser(User currentUser, List<string> likedUsers)
        {
            var dbUsers = await _db.Users
                .Where(u => likedUsers.Contains(u.Id))
                .Take(20)
                .ToListAsync();

            return dbUsers.Select(MapToDomain).Where(u => u != null).ToList()!;
        }

        public async Task<List<User>> GetMatchesForUser(User currentUser, List<string> likedMutualUserIds)
        {
            var dbUsers = await _db.Users
                .Where(u => likedMutualUserIds.Contains(u.Id))
                .Take(20)
                .ToListAsync();

            return dbUsers.Select(MapToDomain).Where(u => u != null).ToList()!;
        }

        public async Task<List<User>> GetRecommendationsForUser(User currentUser, List<string> likedUserIds, List<string> dislikedUserIds)
        {
            // 1. Felhasználó szűrési preferenciáinak betöltése
            var userPref = await _db.UserPreferences.FirstOrDefaultAsync(p => p.UserId == currentUser.Id);

            // Alap lekérdezés: Saját magunk kizárása
            var query = _db.Users
                .Include(u => u.TopSongs)
                    .ThenInclude(ts => ts.Song)
                .Where(u => u.Id != currentUser.Id);

            // Like-olt felhasználók kizárása (ha van a listában elem)
            if (likedUserIds != null && likedUserIds.Any())
            {
                query = query.Where(u => !likedUserIds.Contains(u.Id));
            }

            // Dislike-olt felhasználók kizárása (ha van a listában elem)
            if (dislikedUserIds != null && dislikedUserIds.Any())
            {
                query = query.Where(u => !dislikedUserIds.Contains(u.Id));
            }

            // 2. Szűrők alkalmazása a preferenciák alapján
            if (userPref != null)
            {
                // Nemi preferencia szűrése (kis-/nagybetű függetlenül)
                if (!string.IsNullOrWhiteSpace(userPref.Gender) && !userPref.Gender.Equals("all", StringComparison.OrdinalIgnoreCase))
                {
                    var targetGender = userPref.Gender.ToLower();
                    query = query.Where(u => u.Gender.ToLower() == targetGender);
                }

                // Város szűrése (ha meg van adva, tartalmazza-e a megadott nevet)
                if (!string.IsNullOrWhiteSpace(userPref.City))
                {
                    var targetCity = userPref.City.Trim().ToLower();
                    query = query.Where(u => u.City.ToLower().Contains(targetCity));
                }

                // Életkor szűrése BirthDate alapján
                var today = DateTime.Today;
                var minBirthDate = today.AddYears(-userPref.MaxAge - 1);
                var maxBirthDate = today.AddYears(-userPref.MinAge);

                query = query.Where(u => u.BirthDate >= minBirthDate && u.BirthDate <= maxBirthDate);
            }

            var candidateUsers = await query.Take(50).ToListAsync();

            // Current user top dalai
            var currentUserTopSongs = await _db.UserTopSongs
                .Include(ts => ts.Song)
                .Where(ts => ts.UserId == currentUser.Id)
                .ToListAsync();

            var currentUserTrackIds = currentUserTopSongs.Select(ts => ts.SpotifyTrackId).ToHashSet();
            double[]? currentUserVector = CalculateAverageVector(currentUserTopSongs.Select(ts => ts.Song));

            // 3. Közös dal szűrő (ha a userPref.CommonSongOnly be van kapcsolva)
            if (userPref != null && userPref.CommonSongOnly)
            {
                candidateUsers = candidateUsers
                    .Where(u => u.TopSongs.Any(ts => currentUserTrackIds.Contains(ts.SpotifyTrackId)))
                    .ToList();
            }

            var result = candidateUsers
                .Select(u =>
                {
                    // Közös dalok kigyűjtése
                    var commonSongs = u.TopSongs
                        .Where(ts => currentUserTrackIds.Contains(ts.SpotifyTrackId))
                        .Select(ts => new Song
                        {
                            SpotifyTrackId = ts.Song.SpotifyTrackId,
                            Title = ts.Song.Title,
                            Artist = ts.Song.Artist,
                            PreviewUrl = ts.Song.PreviewUrl,
                            Valence = ts.Song.Valence,
                            Energy = ts.Song.Energy,
                            Danceability = ts.Song.Danceability,
                            Acousticness = ts.Song.Acousticness,
                            NormalizedBpm = ts.Song.NormalizedBpm
                        })
                        .ToList();

                    double[]? targetUserVector = CalculateAverageVector(u.TopSongs.Select(ts => ts.Song));
                    double similarityScore = CalculateSimilarity(currentUserVector, targetUserVector);
                    double finalScore = similarityScore + (commonSongs.Count * 0.25);

                    // DBUser mapping + ajánlóspecifikus mezők beállítása
                    var mappedUser = MapToDomain(u)!;
                    mappedUser.CommonSongs = commonSongs;
                    mappedUser.MatchScore = finalScore;

                    return mappedUser;
                })
                .OrderByDescending(u => u.MatchScore)
                .Take(20)
                .ToList();

            return result;
        }

        public async Task<User?> GetUserById(string userId)
        {
            var currentUser = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            return MapToDomain(currentUser);
        }

        public async Task UpdateProfile(string userId, UserDTO model)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) throw new Exception("User not found");
            if (!string.IsNullOrEmpty(model.FullName))
                user.FullName = model.FullName;
            if (model.Picture != null && model.Picture.Length > 0)
                user.Picture = model.Picture;
            if (!string.IsNullOrEmpty(model.City))
                user.City = model.City;
            if (!string.IsNullOrEmpty(model.Gender))
                user.Gender = model.Gender;
            if (!string.IsNullOrEmpty(model.ShortIntro))
                user.ShortIntro = model.ShortIntro;
            if (!string.IsNullOrEmpty(model.LongIntro))
                user.LongIntro = model.LongIntro;

            _db.Users.Update(user);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateMusicVectorAsync(string userId, string musicVector)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                user.MusicVector = musicVector;
                await _db.SaveChangesAsync();
            }
        }        

        // --- LOGIN & REGISTER ---

        public async Task<User?> FindByEmailAsync(string email)
        {
            var dbUser = await _userManager.FindByEmailAsync(email);
            return MapToDomain(dbUser);
        }

        public async Task<User?> FindByRefreshTokenAsync(string refreshToken)
        {
            var dbUser = await _db.Users
                .FirstOrDefaultAsync(u => _db.UserTokens
                    .Any(t => t.UserId == u.Id && t.Name == "RefreshToken" && t.Value == refreshToken));

            return MapToDomain(dbUser);
        }

        public async Task<IdentityResult> CreateAsync(User user, string password, string spotifyToken)
        {
            var dbUser = new DBUser
            {
                UserName = user.Email,
                Email = user.Email,
                FullName = user.FullName,
                BirthDate = user.BirthDate,
                City = user.City,
                Gender = user.Gender,
                Picture = user.Picture,
                ShortIntro = user.ShortIntro,
                LongIntro = user.LongIntro,
                SpotifyToken = spotifyToken
            };

            var result = await _userManager.CreateAsync(dbUser, password);

            if (result.Succeeded)
            {
                var defaultPreference = new DBUserPreference
                {
                    UserId = dbUser.Id,
                    MinAge = 18,
                    MaxAge = 99,
                    Gender = "all",
                    City = null,
                    CommonSongOnly = false
                };
                _db.UserPreferences.Add(defaultPreference);
                await _db.SaveChangesAsync();
            }

            return result;
        }

        public async Task<IdentityResult> SetRefreshTokenAsync(User user, string refreshToken, DateTime expiry)
        {
            var dbUser = await _userManager.FindByIdAsync(user.Id);

            if (dbUser == null)
            {
                throw new Exception($"FK HIBA ELŐZÉS: A user {user.Id} nem található a DB-ben!");
            }

            var existing = await _db.UserTokens
                .FirstOrDefaultAsync(t => t.UserId == dbUser.Id
                                          && t.LoginProvider == "LocalApp"
                                          && t.Name == "RefreshToken");

            if (existing != null)
            {
                existing.Value = refreshToken;
                _db.UserTokens.Update(existing);
            }
            else
            {
                _db.UserTokens.Add(new IdentityUserToken<string>
                {
                    UserId = dbUser.Id,
                    LoginProvider = "LocalApp",
                    Name = "RefreshToken",
                    Value = refreshToken
                });
            }

            await _db.SaveChangesAsync();

            return IdentityResult.Success;
        }

        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            var dbUser = await _userManager.FindByIdAsync(user.Id);
            return dbUser != null && await _userManager.CheckPasswordAsync(dbUser, password);
        }

        public async Task<IdentityResult> UpdateAsync(User user, string? refreshToken = null, DateTime? expiry = null)
        {
            var dbUser = await _userManager.FindByIdAsync(user.Id);
            if (dbUser == null) return IdentityResult.Failed();

            dbUser.FullName = user.FullName;
            dbUser.City = user.City;
            dbUser.Gender = user.Gender;
            dbUser.ShortIntro = user.ShortIntro;
            dbUser.LongIntro = user.LongIntro;
            dbUser.Picture = user.Picture;

            return await _userManager.UpdateAsync(dbUser);
        }

        // --- SEGÉDFÜGGVÉNYEK ---

        private User? MapToDomain(DBUser? dbUser)
        {
            if (dbUser == null) return null;

            return new User(
                dbUser.Id,
                dbUser.FullName,
                dbUser.Age,
                dbUser.Gender,
                dbUser.City,
                dbUser.Picture,
                dbUser.ShortIntro,
                dbUser.LongIntro)
            {
                Email = dbUser.Email ?? string.Empty,
                BirthDate = dbUser.BirthDate,
                MusicVector = dbUser.MusicVector
            };
        }

        private double CalculateSimilarity(double[]? vectorA, double[]? vectorB)
        {
            if (vectorA == null || vectorB == null || vectorA.Length != vectorB.Length)
                return 0;

            double sumSq = 0;
            for (int i = 0; i < vectorA.Length; i++)
            {
                sumSq += Math.Pow(vectorA[i] - vectorB[i], 2);
            }

            double distance = Math.Sqrt(sumSq);
            return 1.0 / (1.0 + distance);
        }

        private double[]? CalculateAverageVector(IEnumerable<DBSong> songs)
        {
            var songList = songs.ToList();
            if (!songList.Any()) return null;

            double[] avgVector = new double[5];
            foreach (var song in songList)
            {
                avgVector[0] += song.Valence;
                avgVector[1] += song.Energy;
                avgVector[2] += song.Danceability;
                avgVector[3] += song.Acousticness;
                avgVector[4] += song.NormalizedBpm;
            }

            for (int i = 0; i < avgVector.Length; i++)
            {
                avgVector[i] /= songList.Count;
            }

            return avgVector;
        }
    }
}