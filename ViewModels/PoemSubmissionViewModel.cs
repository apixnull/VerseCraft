using System.ComponentModel.DataAnnotations;

namespace VerseCraft.ViewModels
{
    public class PoemSubmissionViewModel
    {
        [Required, MaxLength(200)]
        public string Title { get; set; } = null!;
        // 🏷 e.g., "Ode to the Moon"

        [Required]
        public string Content { get; set; } = null!;
        // 📝 Main poem body

        [MaxLength(300)]
        public string? Summary { get; set; }
        // 🧾 Brief overview

        [MaxLength(100)]
        public string? Genre { get; set; }
        // 🎭 e.g., "Romance"

        [MaxLength(100)]
        public string? Style { get; set; }
        // ✍️ e.g., "Free Verse"

        [MaxLength(100)]
        public string? Language { get; set; }
        // 🌍 e.g., "English"

        [MaxLength(100)]
        public string? Mood { get; set; }
        // 💫 e.g., "Reflective"

        public bool IsPublic { get; set; } = true;
        // 🌐 Visible to public?

        public IFormFile? CoverImage { get; set; }
        // 🖼 Optional file upload
    }
}
