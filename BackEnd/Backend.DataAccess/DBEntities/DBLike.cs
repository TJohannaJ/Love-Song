using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Infrastructure.DBEntities
{
    [Table("Likes")]
    public class DBLike
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }
        public virtual DBUser User { get; set; }
        public string LikedUserId { get; set; }
        public virtual DBUser LikedUser { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Mutual { get; set; }

    }
}
