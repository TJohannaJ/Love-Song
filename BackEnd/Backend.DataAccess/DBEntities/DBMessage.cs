using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Infrastructure.DBEntities
{
    [Table("Messages")]
    public class DBMessage
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string LikeId { get; set; }
        public virtual DBLike Like { get; set; }
        public string SenderId { get; set; }
        public virtual DBUser Sender { get; set; }
        public string ReceiverId { get; set; }
        public virtual DBUser Receiver { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Seen { get; set; }
    }
}
