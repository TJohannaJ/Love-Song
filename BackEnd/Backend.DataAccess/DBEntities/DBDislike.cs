using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Backend.Infrastructure.DBEntities
{
    [Table("Dislikes")]
    public class DBDislike
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string UserId { get; set; }
        public virtual DBUser User { get; set; }
        public string DislikedUserId { get; set; }
        public virtual DBUser DislikedUser { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
