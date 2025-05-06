using System.ComponentModel.DataAnnotations;

namespace VerseCraft.Models
{
    public class Anthology
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Author { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [StringLength(500)]
        public string CopyrightNotice { get; set; } = string.Empty;

        [StringLength(100)]
        public string LicenseType { get; set; } = string.Empty;

        [StringLength(1000)]
        public string TermsAndConditions { get; set; } = string.Empty;

        [StringLength(500)]
        public string ImagePath { get; set; } = string.Empty;
    }
}
