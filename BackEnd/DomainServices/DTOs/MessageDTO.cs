using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Domain.DTOs
{
    public class MessageDTO
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string SenderId { get; set; }
        public string CreatedAt { get; set; }
    }
}
