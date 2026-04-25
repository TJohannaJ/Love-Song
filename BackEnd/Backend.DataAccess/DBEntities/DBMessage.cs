using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Infrastructure.DBEntities
{
    [Table("Messages")]
    public class DBMessage
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string LikeId { get; set; }
        public virtual DBLike Like { get; set; }
        public string SenderId { get; set; }
        public virtual DBUser Sender { get; set; }        
        public DateTime CreatedAt { get; set; }
        public bool Seen { get; set; }
    }
}
