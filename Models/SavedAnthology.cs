using System;
using System.ComponentModel.DataAnnotations;

namespace VerseCraft.Models
{
    public class SavedAnthology
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; } = null!;

        public int AnthologyId { get; set; }
        public virtual Anthology Anthology { get; set; } = null!;

        public DateTime DateSaved { get; set; } = DateTime.UtcNow;
    }
}