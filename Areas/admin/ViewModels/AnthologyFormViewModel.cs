using System.ComponentModel.DataAnnotations;

namespace VerseCraft.Areas.admin.ViewModels
{
    public class AnthologyFormViewModel
    {
        public int? Id { get; set; }  // Null for create, required for edit

        [Required, StringLength(255)]
        public string Title { get; set; } = string.Empty;

        [Required, StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        // Used for displaying existing image
        public string? ExistingImagePath { get; set; }

        [Display(Name = "Cover Image")]
        public IFormFile? CoverImage { get; set; }  // Uploaded file (optional for edit)

        [StringLength(100)]
        [Display(Name = "Author Name")]
        public string AuthorName { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "License Type")]
        public string? LicenseType { get; set; }

        [StringLength(500)]
        [Display(Name = "Copyright Notice")]
        public string? CopyrightNotice { get; set; }

        public int? UserId { get; set; }  // Set internally for authenticated users

        public bool IsPublic { get; set; } 
    }
}
