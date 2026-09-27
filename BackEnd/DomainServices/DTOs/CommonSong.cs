namespace Backend.Domain.DTOs
{
    public class CommonSongDTO
    {
        public string SpotifyTrackId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;
        public string? PreviewUrl { get; set; }
    }
}
