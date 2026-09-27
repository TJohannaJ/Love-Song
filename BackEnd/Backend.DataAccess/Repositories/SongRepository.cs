using Backend.Domain.Entities;
using Backend.Domain.Interfaces;
using Backend.Infrastructure.DBEntities;
using Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories
{
    public class SongRepository : ISongRepository
    {
        private readonly MyDbContext _db;

        public SongRepository(MyDbContext db)
        {
            _db = db;
        }

        public async Task SaveUserTopSongsAsync(string userId, List<(Song Song, int Rank)> songsWithRank)
        {
            // 1. Meglévő top dalok törlése a felhasználóhoz
            var existingTopSongs = await _db.UserTopSongs
                .Where(uts => uts.UserId == userId)
                .ToListAsync();

            if (existingTopSongs.Any())
            {
                _db.UserTopSongs.RemoveRange(existingTopSongs);
                // Elmentjük a törlést, hogy kiürüljön a tranzakciós puffer!
                await _db.SaveChangesAsync();
            }

            foreach (var item in songsWithRank)
            {
                // 2. Mapelés Domain Song-ról DBSong-ra
                var dbSong = await _db.Songs
                    .FirstOrDefaultAsync(s => s.SpotifyTrackId == item.Song.SpotifyTrackId);

                if (dbSong == null)
                {
                    dbSong = new DBSong
                    {
                        SpotifyTrackId = item.Song.SpotifyTrackId,
                        Title = item.Song.Title,
                        Artist = item.Song.Artist,
                        PreviewUrl = item.Song.PreviewUrl,
                        Valence = (float)item.Song.Valence,
                        Energy = (float)item.Song.Energy,
                        Danceability = (float)item.Song.Danceability,
                        Acousticness = (float)item.Song.Acousticness,
                        NormalizedBpm = (float)item.Song.NormalizedBpm
                    };
                    _db.Songs.Add(dbSong);
                }
                else
                {
                    dbSong.Title = item.Song.Title;
                    dbSong.Artist = item.Song.Artist;
                    dbSong.PreviewUrl = item.Song.PreviewUrl;
                    dbSong.Valence = (float)item.Song.Valence;
                    dbSong.Energy = (float)item.Song.Energy;
                    dbSong.Danceability = (float)item.Song.Danceability;
                    dbSong.Acousticness = (float)item.Song.Acousticness;
                    dbSong.NormalizedBpm = (float)item.Song.NormalizedBpm;
                }

                // 3. UserTopSong kapcsoló rekord hozzáadása
                _db.UserTopSongs.Add(new DBUserTopSong
                {
                    UserId = userId,
                    SpotifyTrackId = item.Song.SpotifyTrackId,
                    Rank = item.Rank
                });
            }

            await _db.SaveChangesAsync();
        }

        public async Task<List<Song>> GetUserTopSongsAsync(string userId)
        {
            return await _db.UserTopSongs
                .Where(uts => uts.UserId == userId)
                .OrderBy(uts => uts.Rank)
                .Select(uts => new Song
                {
                    SpotifyTrackId = uts.Song.SpotifyTrackId,
                    Title = uts.Song.Title,
                    Artist = uts.Song.Artist,
                    PreviewUrl = uts.Song.PreviewUrl,
                    Valence = uts.Song.Valence,
                    Energy = uts.Song.Energy,
                    Danceability = uts.Song.Danceability,
                    Acousticness = uts.Song.Acousticness,
                    NormalizedBpm = uts.Song.NormalizedBpm
                })
                .ToListAsync();
        }
    }
}