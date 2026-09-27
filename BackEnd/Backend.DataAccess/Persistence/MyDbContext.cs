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

            // --- LIKES KONFIGURÁCIÓ ---
            modelBuilder.Entity<DBLike>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.LikedUser)
                    .WithMany()
                    .HasForeignKey(e => e.LikedUserId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // --- DISLIKES KONFIGURÁCIÓ ---
            modelBuilder.Entity<DBDislike>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.DislikedUser)
                    .WithMany()
                    .HasForeignKey(e => e.DislikedUserId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<DBMessage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(e => e.Like1)
                      .WithMany()
                      .HasForeignKey(e => e.LikeId1)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Like2)
                      .WithMany()
                      .HasForeignKey(e => e.LikeId2)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Sender)
                      .WithMany()
                      .HasForeignKey(e => e.SenderId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Receiver)
                      .WithMany()
                      .HasForeignKey(e => e.ReceiverId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // --- USER TOP SONGS (N:M Kapcsolótábla) KONFIGURÁCIÓ ---
            modelBuilder.Entity<DBUserTopSong>(entity =>
            {
                // Összetett elsődleges kulcs (UserId + SpotifyTrackId)
                entity.HasKey(uts => new { uts.UserId, uts.SpotifyTrackId });

                entity.HasOne(uts => uts.User)
                    .WithMany(u => u.TopSongs)
                    .HasForeignKey(uts => uts.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(uts => uts.Song)
                    .WithMany(s => s.UserTopSongs)
                    .HasForeignKey(uts => uts.SpotifyTrackId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // --- USER PREFERENCES (1:1 Kapcsolat) KONFIGURÁCIÓ ---
            modelBuilder.Entity<DBUserPreference>(entity =>
            {
                entity.HasKey(p => p.UserId);

                entity.HasOne(p => p.User)
                    .WithOne(u => u.UserPreference)
                    .HasForeignKey<DBUserPreference>(p => p.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        public DbSet<DBUser> Users { get; set; }
        public DbSet<DBLike> Likes { get; set; }
        public DbSet<DBDislike> Dislikes { get; set; }
        public DbSet<DBMessage> Messages { get; set; }
        public DbSet<DBSong> Songs { get; set; }
        public DbSet<DBUserTopSong> UserTopSongs { get; set; }
        public DbSet<DBUserPreference> UserPreferences { get; set; }
    }        
}
