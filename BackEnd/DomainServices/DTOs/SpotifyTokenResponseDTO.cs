using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Backend.Domain.DTOs
{
    public class SpotifyTokenResponseDTO
    {
        [JsonPropertyName("access_token")] // Ez mondja meg a C#-nak, mit keressen a JSON-ben
        public string AccessToken { get; set; }

        [JsonPropertyName("refresh_token")] // EZ A LEGFONTOSABB SOR
        public string RefreshToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
    }
}
