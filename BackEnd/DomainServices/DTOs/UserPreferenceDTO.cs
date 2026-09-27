namespace Backend.Domain.DTOs
{
    public class UserPreferenceDTO
    {
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
        public string? Gender { get; set; }
        public string? City { get; set; }
        public bool? CommonSongOnly { get; set; }
    }
}