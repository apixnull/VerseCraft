using Microsoft.EntityFrameworkCore;
using VerseCraft.Models;

namespace VerseCraft.Data
{
    public class VerseCraftDbContext : DbContext
    {
        public VerseCraftDbContext(DbContextOptions<VerseCraftDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Poem> Poems => Set<Poem>();
        public DbSet<Anthology> Anthologies => Set<Anthology>();
        public DbSet<OTPToken> OTPTokens => Set<OTPToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
                entity.Property(u => u.Name).HasMaxLength(100);
                entity.Property(u => u.PasswordHash).HasMaxLength(255);
            });

            modelBuilder.Entity<Poem>(entity =>
            {
                entity.Property(p => p.Title).IsRequired().HasMaxLength(200);
                entity.Property(p => p.Content).IsRequired();
                entity.HasOne(p => p.User)
                      .WithMany(u => u.Poems)
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(p => p.Anthology)
                      .WithMany(a => a.Poems)
                      .HasForeignKey(p => p.AnthologyId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Anthology>(entity =>
            {
                entity.Property(a => a.Title).IsRequired().HasMaxLength(255);
                entity.Property(a => a.Description).HasMaxLength(1000);
                entity.Property(a => a.ImagePath).HasMaxLength(500);
                entity.HasOne(a => a.User)
                      .WithMany(u => u.Anthologies)
                      .HasForeignKey(a => a.UserId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<OTPToken>(entity =>
            {
                entity.Property(o => o.Token).IsRequired().HasMaxLength(10);
                entity.Property(o => o.Type).IsRequired();
                entity.HasOne(o => o.User)
                      .WithMany(u => u.OTPs)
                      .HasForeignKey(o => o.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Saved Poems (many-to-many)
            modelBuilder.Entity<User>()
                .HasMany(u => u.SavedPoems)
                .WithMany()
                .UsingEntity(j => j.ToTable("UserSavedPoems"));

            // Saved Anthologies (many-to-many)
            modelBuilder.Entity<User>()
                .HasMany(u => u.SavedAnthologies)
                .WithMany()
                .UsingEntity(j => j.ToTable("UserSavedAnthologies"));
        }
    }
}
