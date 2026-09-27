namespace Backend.Domain.Entities
{
    public class UserTopSong
    {
        public string UserId { get; set; } = string.Empty;
        public string SpotifyTrackId { get; set; } = string.Empty;
        public int Rank { get; set; } // 1-10 közötti helyezés
        public Song? Song { get; set; }
        public virtual ICollection<UserTopSong> UserTopSongs { get; set; } = new List<UserTopSong>();
    }
}
