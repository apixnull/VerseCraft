using System.ComponentModel.DataAnnotations;

namespace VerseCraft.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required, MaxLength(500)]
        public string Text { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int PoemId { get; set; }
        public Poem Poem { get; set; } = null!;
    }
}
