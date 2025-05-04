using System.ComponentModel.DataAnnotations;

namespace VerseCraft.Models
{
    public class Poem
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = null!;

        [Required]
        public string Content { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsPublic { get; set; } = true;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }

}
