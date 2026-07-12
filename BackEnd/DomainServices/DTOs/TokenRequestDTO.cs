using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Domain.DTOs
{
    public class TokenRequestDTO
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
