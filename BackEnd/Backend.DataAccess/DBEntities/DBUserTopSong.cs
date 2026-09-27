using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Infrastructure.DBEntities
{
    [Table("UserTopSongs")]
    public class DBUserTopSong
    {
        public string UserId { get; set; }
        public virtual DBUser User { get; set; }
        public string SpotifyTrackId { get; set; }
        public virtual DBSong Song { get; set; }        
        public int Rank { get; set; }
    }
}
