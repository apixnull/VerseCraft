using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VerseCraft.Models
{
    // Represents an application user, including their poems, anthologies, OTP tokens, and saved items.
    public class User
    {
        [Key]
        public int Id { get; set; }

        // The user's display name (optional).
        [MaxLength(100)]
        public string? Name { get; set; }

        // The user's email address (required, unique).
        [Required, MaxLength(150)]
        public string Email { get; set; } = null!;

        // The hashed password (optional, for local accounts).
        [MaxLength(255)]
        public string? PasswordHash { get; set; }

        // Whether the user's email is verified.
        public bool IsVerified { get; set; } = false;

        // Whether the user has admin privileges.
        public bool IsAdmin { get; set; } = false;

        // The date and time the user was created (UTC).
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ============================
        // Relationships
        // ============================

        // Poems authored by this user.
        public virtual ICollection<Poem> Poems { get; set; } = new List<Poem>();

        // Anthologies created by this user.
        public virtual ICollection<Anthology> Anthologies { get; set; } = new List<Anthology>();

        // OTP tokens associated with this user.
        public virtual ICollection<OTPToken> OTPs { get; set; } = new List<OTPToken>();

        // Poems saved by this user (many-to-many with extra data).
        public virtual ICollection<SavedPoem> SavedPoems { get; set; } = new List<SavedPoem>();

        // Anthologies saved by this user (many-to-many with extra data).
        public virtual ICollection<SavedAnthology> SavedAnthologies { get; set; } = new List<SavedAnthology>();
    }
}