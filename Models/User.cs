using System.ComponentModel.DataAnnotations;

namespace VerseCraft.Models
{
    public class User
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string? Name { get; set; }

        [Required, MaxLength(150)]
        public string Email { get; set; } = null!;

        [MaxLength(255)]
        public string? PasswordHash { get; set; }

        public bool IsVerified { get; set; } = false;
        public bool IsAdmin { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Poem> Poems { get; set; } = new List<Poem>(); // Existing relationship

        public ICollection<Anthology> Anthologies { get; set; } = new List<Anthology>(); // Add this for Anthologies

        public ICollection<OTPToken> OTPs { get; set; } = new List<OTPToken>();

        public ICollection<Poem> SavedPoems { get; set; } = new List<Poem>(); // Saved poems

        public ICollection<Anthology> SavedAnthologies { get; set; } = new List<Anthology>(); // Saved anthologies
    }
}
