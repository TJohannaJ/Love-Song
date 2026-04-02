using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Domain.Entities
{
    public class User 
    {       
        public string Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public int Age { get; set; }        
        public string Gender { get; set; }
        public string City { get; set; }
        public string Preferences { get; set; }
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
            Preferences = preferences;
            Picture = picture;
            ShortIntro = shortIntro;
            LongIntro = longIntro;
        }
    }
}
