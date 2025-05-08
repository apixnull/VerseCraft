using System.ComponentModel.DataAnnotations;

namespace VerseCraft.Areas.admin.ViewModels
{
    public class PoemFormViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title must be less than 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Summary can't exceed 500 characters")]
        public string? Summary { get; set; }

        [StringLength(100)]
        public string? Genre { get; set; }

        [StringLength(100)]
        public string? Style { get; set; }

        [StringLength(100)]
        public string? Language { get; set; }

        [StringLength(100)]
        public string? Mood { get; set; }

        public bool IsPublic { get; set; } = true;
        public bool IsApproved { get; set; } = false;
        public bool IsFeatured { get; set; } = false;

        [Display(Name = "Cover Image")]
        public IFormFile? CoverImageFile { get; set; }

        // New property to store the existing image path (for edit operations)
        [Display(Name = "Existing Cover Image")]
        public string? ExistingCoverImagePath { get; set; }

        [Display(Name = "Author (if not linked to user)")]
        [StringLength(100)]
        public string? Author { get; set; }

        [Display(Name = "User")]
        public int? UserId { get; set; }

        [Display(Name = "Anthology")]
        public int? AnthologyId { get; set; }
    }
}
