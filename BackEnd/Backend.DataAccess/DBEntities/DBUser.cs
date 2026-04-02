using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Backend.Infrastructure.DBEntities
{
    [Table("Users")]
    public class DBUser : IdentityUser
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string SpotifyToken { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public int Age
        {
            get {
                var today = DateTime.Today;
                var age = today.Year - BirthDate.Year;
                if (BirthDate.Date > today.AddYears(-age)) age--;
                return age;
            }
            
        }
        public string Gender { get; set; }
        public string City { get; set; }
        public string Preference { get; set; }
        public string ShortIntro { get; set; }
        public string LongIntro { get; set; }
        public byte[] Picture { get; set; }
    }
}
