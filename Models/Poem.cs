using System;
using System.ComponentModel.DataAnnotations;

namespace VerseCraft.Models
{
    // Represents a poem, including its content, metadata, and relationships.
    public class Poem
    {
        [Key]
        public int Id { get; set; }

        // ============================
        // 📘 Main Information
        // ============================

        // The title of the poem (required, max 200 chars).
        [Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;

        // The main content of the poem (required).
        [Required]
        public string Content { get; set; } = string.Empty;

        // A short summary of the poem (optional, max 500 chars).
        [StringLength(500)]
        public string? Summary { get; set; }

        // The genre of the poem (optional, max 100 chars).
        [StringLength(100)]
        public string? Genre { get; set; }

        // The style of the poem (optional, max 100 chars).
        [StringLength(100)]
        public string? Style { get; set; }

        // The theme of the poem (optional, max 50 chars).
        [StringLength(50)]
        public string? Theme { get; set; }

        // Comma-separated tags for the poem (optional, max 500 chars).
        [StringLength(500)]
        public string? Tags { get; set; }

        // The language of the poem (optional, max 100 chars).
        [StringLength(100)]
        public string? Language { get; set; }

        // The mood of the poem (optional, max 100 chars).
        [StringLength(100)]
        public string? Mood { get; set; }

        // The license type for the poem (optional, max 100 chars).
        [StringLength(100)]
        public string? LicenseType { get; set; }

        // Copyright notice (optional, max 500 chars).
        [StringLength(500)]
        public string? CopyrightNotice { get; set; }

        // Path to the cover image (optional, max 200 chars).
        [StringLength(200)]
        public string? CoverImagePath { get; set; }

        // The author's display name (required, max 100 chars).
        [Required, StringLength(100)]
        public string AuthorName { get; set; } = string.Empty;

        // ============================
        // 📅 Metadata
        // ============================

        // The date and time the poem was created (UTC).
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // The date and time the poem was last updated (UTC, optional).
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

        // ============================
        // 📜 Approval & Visibility
        // ============================

        // Whether the poem is approved (default is false).
        public bool IsApproved { get; set; } = false;

        // Whether the poem is public (default is false).
        public bool IsPublic { get; set; } = false;

        // ============================
        // 📚 Relationships
        // ============================

        // The ID of the user who authored the poem (optional).
        public int? UserId { get; set; }

        // The user who authored the poem (optional).
        public virtual User? User { get; set; }

        // The anthology this poem belongs to (optional).
        public virtual ICollection<AnthologyPoem> AnthologyPoems { get; set; } = new List<AnthologyPoem>();
    }
}
