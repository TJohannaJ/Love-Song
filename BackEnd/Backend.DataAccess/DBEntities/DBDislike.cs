using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Infrastructure.DBEntities
{
    [Table("Dislikes")]
    public class DBDislike
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }
        public virtual DBUser User { get; set; }
        public string DislikedUserId { get; set; }
        public virtual DBUser DislikedUser { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
