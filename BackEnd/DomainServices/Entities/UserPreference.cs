using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Domain.Entities
{
    public class UserPreference
    {
        public string UserId { get; set; } = string.Empty;
        public int MinAge { get; set; } = 18;
        public int MaxAge { get; set; } = 99;
        public string Gender { get; set; } = "all";
        public string? City { get; set; }
        public bool CommonSongOnly { get; set; } = false;

        public UserPreference() { }

        public UserPreference(string userId, int minAge, int maxAge, string gender, string? city, bool commonSongOnly)
        {
            UserId = userId;
            MinAge = minAge;
            MaxAge = maxAge;
            Gender = gender;
            City = city;
            CommonSongOnly = commonSongOnly;
        }
    }
}
