using System.ComponentModel.DataAnnotations;

namespace VerseCraft.ViewModels
{
    public class VerifyOtpViewModel
    {
        public int UserId { get; set; }

        public string Otp { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty; // Add this property to hold error messages
    }
}
