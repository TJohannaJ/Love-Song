using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Infrastructure.DBEntities
{
    [Table("UserPreferences")]
    public class DBUserPreference
    {
        [Key]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;

        public int MinAge { get; set; } = 18;

        public int MaxAge { get; set; } = 99;

        [MaxLength(20)]
        public string Gender { get; set; } = "all";

        [MaxLength(100)]
        public string? City { get; set; }

        public bool CommonSongOnly { get; set; } = false;

        // Navigációs tulajdonság
        public virtual DBUser User { get; set; } = null!;
    }
}