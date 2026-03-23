using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Backend.Infrastructure.DBEntities
{
    [Table("Messages")]
    public class DBMessage
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int LikesId { get; set; }
        public DBLike Like { get; set; }
        public int SenderId { get; set; }
        public DBUser Sender { get; set; }        
        public DateTime CreatedAt { get; set; }
        public bool Seen { get; set; }
    }
}
