using Backend.Infrastructure.DBEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Persistence
{
    public class MyDbContext : IdentityDbContext<DBUser>
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DBDislike>()
                .HasOne(d => d.DislikedUser) 
                .WithMany()
                .HasForeignKey(d => d.DislikedUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<DBLike>()
                .HasOne(l => l.LikedUser)
                .WithMany()
                .HasForeignKey(l => l.LikedUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<DBMessage>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.NoAction);
        }

        public DbSet<DBUser> Users { get; set; }
        public DbSet<DBLike> Likes { get; set; }
        public DbSet<DBDislike> Dislikes { get; set; }
        public DbSet<DBMessage> Messages { get; set; }
    }        
}
