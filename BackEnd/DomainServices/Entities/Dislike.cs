using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Domain.Entities
{
    public class Dislike
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string LikedUserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
