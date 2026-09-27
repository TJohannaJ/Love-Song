using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Infrastructure.DBEntities
{
    [Table("Songs")]
    public class DBSong
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string SpotifyTrackId { get; set; } 

        [Required]
        public string Title { get; set; }
        public string Artist { get; set; }
        public string PreviewUrl { get; set; }
        public float Valence { get; set; }
        public float Energy { get; set; }
        public float Danceability { get; set; }
        public float Acousticness { get; set; }
        public float NormalizedBpm { get; set; }

        
        public virtual ICollection<DBUserTopSong> UserTopSongs { get; set; } = new List<DBUserTopSong>();
    }
}
