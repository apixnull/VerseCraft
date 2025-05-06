using System.ComponentModel.DataAnnotations;

namespace VerseCraft.ViewModels
{
    public class ContactFormViewModel
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Content { get; set; } = string.Empty;
    }
}
