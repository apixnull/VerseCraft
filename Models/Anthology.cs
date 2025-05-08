using System.ComponentModel.DataAnnotations;

namespace VerseCraft.Models
{
    public class Anthology
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(255)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [StringLength(500)]
        public string? ImagePath { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }


        [StringLength(100)]
        public string? LicenseType { get; set; }

        [StringLength(500)]
        public string? CopyrightNotice { get; set; }

        public bool IsFeatured { get; set; } = false;

        public bool IsPublic { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


        // Foreign key to User (author/creator of the anthology)
        public int? UserId { get; set; }  // Nullable, since not all anthologies need a user

        public virtual User? User { get; set; }  // Navigation property to User

        public ICollection<Poem> Poems { get; set; } = new List<Poem>();
    }
}
