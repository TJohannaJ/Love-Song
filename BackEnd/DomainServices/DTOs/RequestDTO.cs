using System.Text.Json.Serialization;

namespace Backend.Domain.DTOs
{
    public class RequestDTO
    {
        [JsonPropertyName("requestedUserId")]
        public string RequestedUserId { get; set; }
    }
}
