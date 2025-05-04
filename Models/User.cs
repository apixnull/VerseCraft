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

        public ICollection<Poem> Poems { get; set; } = new List<Poem>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<OTPToken> OTPs { get; set; } = new List<OTPToken>();
    }

}
