using Backend.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Domain.Interfaces
{
	public interface ISpotifyService
	{
		string GetLoginUrl(string userId);
		Task<bool> ExchangeCodeForTokensAsync(string code, string userId);
		Task<IEnumerable<Song>> GetSavedUserTopSongsAsync(string userId);
		Task<bool> SyncUserTopTracksAsync(string userId, int limit = 10);
		Task<Song> CreateSongEntityAsync(string spotifyTrackId, string title, string artist, string? spotifyPreviewUrl);
	}
}