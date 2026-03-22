using Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace Backend.Infrastructure.Persistence
{
    public class MyDbContext : DbContext
    {
        public DbSet<TempUser> Users { get; set; }
        public DbSet<TempLike> Likes { get; set; }
        public DbSet<TempDisLike> Dislikes { get; set; }
    }

    public class TempUser
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Email { get; set; }
        public string Password { get; set; }
        public string SpotifyToken { get; set; }
        public string Name { get; set; }        
        public int Age { get; set; }
        public string Gender { get; set; }
        public string City { get; set; }     
        public string Preference { get; set; }
        public string ShortIntro { get; set; }
        public string LongIntro { get; set; }
        public byte[] Picture { get; set; }
    }

    public class TempLike
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public TempUser User { get; set; }
        [Required]
        public TempUser LikedUser { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Mutual { get; set; }
    }

    public class TempDisLike
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public TempUser User { get; set; }
        [Required]
        public TempUser DislikedUser { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Mutual { get; set; }
    }

        
}
