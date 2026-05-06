using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Infrastructure.DBEntities
{
    [Table("Messages")]
    public class DBMessage
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string LikeId1 { get; set; }
        [ForeignKey("LikeId1")]
        public virtual DBLike Like1 { get; set; }
        [Required]
        public string LikeId2 { get; set; }
        [ForeignKey("LikeId2")]
        public virtual DBLike Like2 { get; set; }
        [Required]
        public string SenderId { get; set; }
        [ForeignKey("SenderId")]
        public virtual DBUser Sender { get; set; }
        [Required]
        public string ReceiverId { get; set; }
        [ForeignKey("ReceiverId")]
        public virtual DBUser Receiver { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Seen { get; set; }
    }
}
