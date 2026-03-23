using Backend.Domain.Entities;
using Backend.Infrastructure.DBEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace Backend.Infrastructure.Persistence
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }

        public DbSet<DBUser> Users { get; set; }
        public DbSet<DBLike> Likes { get; set; }
        public DbSet<DBDislike> Dislikes { get; set; }
        public DbSet<DBMessage> Messages { get; set; }
    }        
}
