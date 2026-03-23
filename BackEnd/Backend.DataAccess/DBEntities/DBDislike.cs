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
        public int UserId { get; set; }
        public DBUser User { get; set; }
        public int DislikedUserId { get; set; }
        public DBUser DislikedUser { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Mutual { get; set; }
    }
}
