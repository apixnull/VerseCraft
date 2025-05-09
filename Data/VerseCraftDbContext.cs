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
        public DbSet<ApprovedItem> ApprovedItems => Set<ApprovedItem>();
        public DbSet<PublicItem> PublicItems => Set<PublicItem>();
        public DbSet<FeaturedItem> FeaturedItems => Set<FeaturedItem>();
        public DbSet<SavedPoem> SavedPoems => Set<SavedPoem>();
        public DbSet<SavedAnthology> SavedAnthologies => Set<SavedAnthology>();

        public DbSet<AnthologyPoem> AnthologyPoems { get; set; } // 👈 Add this
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
                entity.Property(u => u.Name).HasMaxLength(100);
                entity.Property(u => u.PasswordHash).HasMaxLength(255);
            });

            // Poem
            modelBuilder.Entity<Poem>(entity =>
            {
                entity.Property(p => p.Title).IsRequired().HasMaxLength(200);
                entity.Property(p => p.Content).IsRequired();

                entity.HasOne(p => p.User)
                      .WithMany(u => u.Poems)
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

            });

            // Anthology
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

            // OTPToken
            modelBuilder.Entity<OTPToken>(entity =>
            {
                entity.Property(o => o.Token).IsRequired().HasMaxLength(10);
                entity.Property(o => o.Type).IsRequired();

                entity.HasOne(o => o.User)
                      .WithMany(u => u.OTPs)
                      .HasForeignKey(o => o.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ApprovedItem
            modelBuilder.Entity<ApprovedItem>(entity =>
            {
                entity.Property(a => a.WorkType).IsRequired().HasMaxLength(50);

                entity.HasOne(a => a.Poem)
                      .WithMany()
                      .HasForeignKey(a => a.PoemId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(a => a.Anthology)
                      .WithMany()
                      .HasForeignKey(a => a.AnthologyId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // PublicItem
            modelBuilder.Entity<PublicItem>(entity =>
            {
                entity.Property(p => p.WorkType).IsRequired().HasMaxLength(50);

                entity.HasOne(p => p.Poem)
                      .WithMany()
                      .HasForeignKey(p => p.PoemId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(p => p.Anthology)
                      .WithMany()
                      .HasForeignKey(p => p.AnthologyId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // FeaturedItem
            modelBuilder.Entity<FeaturedItem>(entity =>
            {
                entity.Property(f => f.WorkType).IsRequired().HasMaxLength(50);

                entity.HasOne(f => f.Poem)
                      .WithMany()
                      .HasForeignKey(f => f.PoemId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(f => f.Anthology)
                      .WithMany()
                      .HasForeignKey(f => f.AnthologyId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // SavedPoem
            modelBuilder.Entity<SavedPoem>(entity =>
            {
                entity.HasKey(sp => new { sp.UserId, sp.PoemId });

                entity.HasOne(sp => sp.User)
                      .WithMany(u => u.SavedPoems)
                      .HasForeignKey(sp => sp.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(sp => sp.Poem)
                      .WithMany()
                      .HasForeignKey(sp => sp.PoemId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // SavedAnthology
            modelBuilder.Entity<SavedAnthology>(entity =>
            {
                entity.HasKey(sa => new { sa.UserId, sa.AnthologyId });

                entity.HasOne(sa => sa.User)
                      .WithMany(u => u.SavedAnthologies)
                      .HasForeignKey(sa => sa.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(sa => sa.Anthology)
                      .WithMany()
                      .HasForeignKey(sa => sa.AnthologyId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // AnthologyPoem many-to-many relationship configuration
            modelBuilder.Entity<AnthologyPoem>()
           .HasKey(ap => new { ap.AnthologyId, ap.PoemId });

            modelBuilder.Entity<AnthologyPoem>()
          .HasOne(ap => ap.Anthology)
          .WithMany(a => a.AnthologyPoems)
          .HasForeignKey(ap => ap.AnthologyId);

            modelBuilder.Entity<AnthologyPoem>()
          .HasOne(ap => ap.Poem)
          .WithMany(p => p.AnthologyPoems)
          .HasForeignKey(ap => ap.PoemId);
        }
    }
}
