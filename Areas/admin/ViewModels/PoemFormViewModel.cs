using System.ComponentModel.DataAnnotations;


namespace VerseCraft.Areas.admin.ViewModels
{
    public class PoemFormViewModel
    {
        public int? Id { get; set; } // Null for create, set for edit

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

        [StringLength(50)]
        public string? Theme { get; set; }

        [StringLength(500)]
        public string? Tags { get; set; }

        [StringLength(100)]
        public string? Language { get; set; }

        [StringLength(100)]
        public string? Mood { get; set; }

        [StringLength(100)]
        public string? LicenseType { get; set; }

        [StringLength(500)]
        public string? CopyrightNotice { get; set; }

        // this is for existing
        [StringLength(200)]
        public string? CoverImagePath { get; set; } 

        // Allows uploading a new cover image file
        [Display(Name = "New Cover Image")]
        public IFormFile? NewCoverImagePath { get; set; }

        [Required, StringLength(100)]
        public string AuthorName { get; set; } = string.Empty;

        public int? UserId { get; set; }
    }
}
