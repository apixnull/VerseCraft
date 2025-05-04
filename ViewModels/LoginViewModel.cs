using System.ComponentModel.DataAnnotations;

namespace VerseCraft.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(150, ErrorMessage = "Email should not exceed 150 characters.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password should be between 8 and 100 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
