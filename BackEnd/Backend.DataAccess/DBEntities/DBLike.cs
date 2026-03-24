using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Backend.Infrastructure.DBEntities
{
    [Table("Likes")]
    public class DBLike
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int UserId { get; set; }
        public virtual DBUser User { get; set; }
        public int LikedUserId { get; set; }
        public virtual DBUser LikedUser { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Mutual { get; set; }

    }
}
