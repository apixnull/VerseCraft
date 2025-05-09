using System.ComponentModel.DataAnnotations;

namespace VerseCraft.Models
{
    public class SavedPoem
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; } = null!;

        public int PoemId { get; set; }
        public virtual Poem Poem { get; set; } = null!;

        public DateTime DateSaved { get; set; } = DateTime.UtcNow;
    }
}
