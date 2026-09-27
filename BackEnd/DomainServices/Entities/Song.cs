
namespace Backend.Domain.Entities
{
    public class Song
    {
        public string SpotifyTrackId { get; set; } = string.Empty; 
        public string Title { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;
        public string? PreviewUrl { get; set; }

        public double Valence { get; set; }     
        public double Energy { get; set; }      
        public double Danceability { get; set; } 
        public double Acousticness { get; set; } 
        public double NormalizedBpm { get; set; }
        public virtual ICollection<UserTopSong> UserTopSongs { get; set; } = new List<UserTopSong>();
        public double[] ToVector() => new[] { Valence, Energy, Danceability, Acousticness, NormalizedBpm };
    }
}
