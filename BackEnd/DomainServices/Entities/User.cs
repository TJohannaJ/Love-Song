using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Domain.Entities
{
    public class Recommendation
    {
        
        public Guid Id { get; set; }
        public string FullName { get; set; } 
        public int Age { get; set; }
        public string Gender { get; set; }
        public string City { get; set; }
        public string Preferences { get; set; }
        public byte[] Picture { get; set; }
        public string ShortIntro { get; set; }
        public string LongIntro { get; set; }

        public bool Liked { get; set; } = false;
        public bool Disliked { get; set; } = false;

        public Recommendation(string fullname, int age, string gender, string city, string preferences, byte[] picture, string shortIntro, string longIntro, bool liked, bool disliked)
        {
            Id = Guid.NewGuid();
            FullName = fullname;
            Age = age;
            Gender = gender;
            City = city;
            Preferences = preferences;
            Picture = picture;
            ShortIntro = shortIntro;
            LongIntro = longIntro;
            Liked = liked;
            Disliked = disliked;
        }
    }
}
