using System.ComponentModel.DataAnnotations;

namespace VerseCraft.Models
{
    public class PublicItem
    {
        [Key]
        public int Id { get; set; }

        // The type of item that is public (Poem or Anthology).
        [Required]
        public WorkType WorkType { get; set; }

        public int? PoemId { get; set; }
        public virtual Poem? Poem { get; set; }

        public int? AnthologyId { get; set; }
        public virtual Anthology? Anthology { get; set; }
    }
}