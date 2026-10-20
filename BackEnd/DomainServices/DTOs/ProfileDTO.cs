namespace Backend.Domain.DTOs
{
    public class ProfileDTO //ProfileCard-okhoz
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public int Age { get; set; }
        public string Picture { get; set; }
        public string City { get; set; }
        public string Gender { get; set; }
        public string ShortIntro { get; set; }
        public string LongIntro { get; set; }
        public List<CommonSongDTO> CommonSongs { get; set; } = new();
    }
}
