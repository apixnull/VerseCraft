using System.ComponentModel.DataAnnotations;

namespace VerseCraft.Models
{
    // Represents a featured item, which can be either a poem or an anthology.
    public class FeaturedItem
    {
        [Key]
        public int Id { get; set; }

        // The type of item featured (Poem or Anthology).
        [Required]
        public WorkType WorkType { get; set; }

        // The ID of the featured poem (if applicable).
        public int? PoemId { get; set; }

        // The featured poem (if applicable).
        public virtual Poem? Poem { get; set; }

        // The ID of the featured anthology (if applicable).
        public int? AnthologyId { get; set; }

        // The featured anthology (if applicable).
        public virtual Anthology? Anthology { get; set; }
    }
}
