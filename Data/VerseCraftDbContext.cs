using Microsoft.EntityFrameworkCore;
using VerseCraft.Models;

namespace VerseCraft.Data
{
    public class VerseCraftDbContext : DbContext
    {
        public VerseCraftDbContext(DbContextOptions<VerseCraftDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Poem> Poems => Set<Poem>();
        public DbSet<Comment> Comments => Set<Comment>();
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
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.Property(c => c.Text).IsRequired().HasMaxLength(500);
                entity.HasOne(c => c.User)
                      .WithMany(u => u.Comments)
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(c => c.Poem)
                      .WithMany(p => p.Comments)
                      .HasForeignKey(c => c.PoemId)
                      .OnDelete(DeleteBehavior.Cascade);
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
        }
    }
}
