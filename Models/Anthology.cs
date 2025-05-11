using System.ComponentModel.DataAnnotations;

namespace VerseCraft.Models
{
    // Represents a collection of poems (anthology), including metadata and relationships.
    public class Anthology
    {
        [Key]
        public int Id { get; set; }

        // ============================
        // 📘 Main Information
        // ============================

        // The title of the anthology (required, max 255 chars).
        [Required, StringLength(255)]
        public string Title { get; set; } = string.Empty;

        // A description of the anthology (required, max 1000 chars).
        [Required, StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        // Path to the cover image (required, max 500 chars).
        [Required, StringLength(500)]
        public string ImagePath { get; set; } = string.Empty;

        // The author's display name (required, max 100 chars).
        [StringLength(100)]
        public string AuthorName { get; set; } = string.Empty;

        // The license type for the anthology (optional, max 100 chars).
        [StringLength(100)]
        public string? LicenseType { get; set; }

        // Copyright notice (optional, max 500 chars).
        [StringLength(500)]
        public string? CopyrightNotice { get; set; }

        // ============================
        // 👤 Ownership (Optional)
        // ============================

        // The ID of the user who created the anthology (optional).
        public int? UserId { get; set; }

        // The user who created the anthology (optional).
        public virtual User? User { get; set; }

        // ============================
        // 📅 Metadata
        // ============================

        // The date and time the anthology was created (UTC).
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // The date and time the anthology was last updated (UTC).
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Whether the anthology is public (default is false).
        public bool IsPublic { get; set; } = false;

        // Whether the anthology is approved (default is false).
        public bool IsApproved { get; set; } = false;

        // ============================
        // 📚 Relationships
        // ============================
        public virtual ICollection<AnthologyPoem> AnthologyPoems { get; set; } = new List<AnthologyPoem>();
    }
}
