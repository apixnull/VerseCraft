using System.ComponentModel.DataAnnotations;

namespace VerseCraft.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        // Indicates whether the reset link was sent
        public bool Sent { get; set; } = false;
    }
}
