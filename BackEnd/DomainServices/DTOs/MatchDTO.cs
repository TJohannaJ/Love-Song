using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Domain.DTOs
{
    public class MatchDTO
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public int Age { get; set; }  
        public byte[] Picture { get; set; }
        public string LastMessage { get; set; }
        public string LastMessageTime { get; set; }
    }
}
