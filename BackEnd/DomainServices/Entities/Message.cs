using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Domain.Entities
{
    public class Message
    {
        public string Id { get; set; }
        public string LikeId { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Seen { get; set; }
    }
}
