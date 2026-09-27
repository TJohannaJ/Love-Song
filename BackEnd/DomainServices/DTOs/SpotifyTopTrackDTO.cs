using System.Collections.Generic;

namespace Backend.Domain.DTOs
{
    public class SpotifyTopTrackDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string AlbumName { get; set; }
        public List<string> Artists { get; set; }
        public string PreviewUrl { get; set; }
        public string ExternalUrl { get; set; }
    }
}
