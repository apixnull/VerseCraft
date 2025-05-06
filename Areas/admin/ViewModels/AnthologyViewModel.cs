using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace VerseCraft.Areas.Admin.ViewModels
{
    public class AnthologyViewModel
    {
        public int Id { get; set; } // ✅ For updating existing anthology    

        [Required(ErrorMessage = "The title is required.")]
        [StringLength(255, ErrorMessage = "The title must be 255 characters or less.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "The content is required.")]
        [MinLength(20, ErrorMessage = "The content must be at least 20 characters long.")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Author name is required.")]
        [StringLength(100, ErrorMessage = "Author name must be 100 characters or less.")]
        public string Author { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Copyright notice must be 500 characters or less.")]
        public string? CopyrightNotice { get; set; }

        [StringLength(100, ErrorMessage = "License type must be 100 characters or less.")]
        public string? LicenseType { get; set; }

        [StringLength(1000, ErrorMessage = "Terms and conditions must be 1000 characters or less.")]
        public string? TermsAndConditions { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile? ImageFile { get; set; }

        public string? ExistingImagePath { get; set; } // ✅ For displaying existing image on update
    }
}
