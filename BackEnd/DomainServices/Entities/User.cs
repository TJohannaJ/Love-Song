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
        public string Preference { get; set; }
        public string ShortIntro { get; set; }
        public string LongIntro { get; set; }
        public byte[] Picture { get; set; }       

        public User() { }
        public User(string id, string fullname, int age, string gender, string city, string preferences, byte[] picture, string shortIntro, string longIntro)
        {
            Id = id;
            FullName = fullname;
            Age = age;
            Gender = gender;
            City = city;
            Preference = preferences;
            Picture = picture;
            ShortIntro = shortIntro;
            LongIntro = longIntro;
        }
    }
}
