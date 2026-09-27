using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Infrastructure.DBEntities
{
    [Table("Users")]
    public class DBUser : IdentityUser
    {
        public string SpotifyToken { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public int Age
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - BirthDate.Year;
                if (BirthDate.Date > today.AddYears(-age)) age--;
                return age;
            }

        }
        public string Gender { get; set; }
        public string City { get; set; }
        public string ShortIntro { get; set; }
        public string LongIntro { get; set; }
        public byte[] Picture { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        public string? MusicVector { get; set; }
        public virtual ICollection<DBUserTopSong> TopSongs { get; set; } = new List<DBUserTopSong>();
        public virtual DBUserPreference? UserPreference { get; set; }
    }
}
