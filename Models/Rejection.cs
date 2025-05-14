
using System.ComponentModel.DataAnnotations;

namespace VerseCraft.Models
{
    public class Rejection
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ItemId { get; set; } // ID of the rejected item

        [Required, StringLength(50)]
        public string ItemType { get; set; } = string.Empty; // "Poem" or "Anthology"

        public string RejectionReason { get; set; } = string.Empty; // Optional reason for rejection

        [Required]
        public DateTime RejectedAt { get; set; } = DateTime.UtcNow;

    }
}