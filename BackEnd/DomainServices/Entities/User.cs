namespace Backend.Domain.Entities
{
    public class User 
    {       
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; } 
        public DateTime BirthDate { get; set; }
        public int Age { get; set; }        
        public string Gender { get; set; }
        public string City { get; set; }
        public string ShortIntro { get; set; }
        public string LongIntro { get; set; }
        public byte[] Picture { get; set; }
        public string? MusicVector { get; set; }
        public List<UserTopSong> TopSongs { get; set; } = new();
        public List<Song> CommonSongs { get; set; } = new();
        public double MatchScore { get; set; }
        public UserPreference? UserPreference { get; set; }

        public User() { }
        public User(string id, string fullname, int age, string gender, string city, byte[] picture, string shortIntro, string longIntro)
        {
            Id = id;
            FullName = fullname;
            Age = age;
            Gender = gender;
            City = city;
            Picture = picture;
            ShortIntro = shortIntro;
            LongIntro = longIntro;
        }
    }
}
