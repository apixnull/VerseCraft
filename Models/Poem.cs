using System;
using System.ComponentModel.DataAnnotations;

namespace VerseCraft.Models
{
    public class Poem
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Summary { get; set; }

        [StringLength(100)]
        public string? Genre { get; set; }

        [StringLength(100)]
        public string? Style { get; set; }

        [StringLength(100)]
        public string? Language { get; set; }

        [StringLength(100)]
        public string? Mood { get; set; }

        [StringLength(200)]
        public string? CoverImagePath { get; set; }

        public bool IsPublic { get; set; } = true;

        public bool IsApproved { get; set; } = false;

        public bool IsFeatured { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public string Author { get; set; } = string.Empty; // Poem author as a string

        public int? UserId { get; set; } // Nullable UserId, if the poem is not owned by a user, it can be null

        public virtual User? User { get; set; }  // Navigation property to User
        public int? AnthologyId { get; set; } // Optional reference to the Anthology

        public virtual Anthology? Anthology { get; set; } // Optional navigation property for easier access to Anthology
    }
}
