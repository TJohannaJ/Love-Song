using Backend.Domain.Entities;

namespace Backend.Domain.Interfaces
{
    public interface ISongRepository
    {
        Task SaveUserTopSongsAsync(string userId, List<(Song Song, int Rank)> songsWithRank);
        Task<List<Song>> GetUserTopSongsAsync(string userId);
    }
}
