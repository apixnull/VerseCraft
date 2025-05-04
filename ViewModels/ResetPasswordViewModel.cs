using System.ComponentModel.DataAnnotations;

namespace VerseCraft.ViewModels
{
    public class ResetPasswordViewModel
    {
        public int UserId { get; set; }
        public string Token { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
