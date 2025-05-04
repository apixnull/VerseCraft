using System.ComponentModel.DataAnnotations;

namespace VerseCraft.Models
{
    public class OTPToken
    {
        public int Id { get; set; }

        [Required, MaxLength(10)]
        public string Token { get; set; } = null!;

        public OTPType Type { get; set; }
        public DateTime Expiry { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }

    public enum OTPType
    {
        AccountVerification = 0,
        PasswordReset = 1
    }
}
