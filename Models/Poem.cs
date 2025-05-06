using System.ComponentModel.DataAnnotations;
using VerseCraft.Models;

public class Poem
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = null!;
    // e.g., "Ode to the Moon"

    [Required]
    public string Content { get; set; } = null!;
    // Full poem content

    [MaxLength(300)]
    public string? Summary { get; set; }
    // e.g., "A lyrical meditation on solitude and nighttime wonder."

    [MaxLength(100)]
    public string? Genre { get; set; }
    // e.g., "Romance", "Nature", "Spiritual"

    [MaxLength(100)]
    public string? Style { get; set; }
    // e.g., "Haiku", "Free Verse", "Sonnet"

    [MaxLength(100)]
    public string? Language { get; set; }
    // e.g., "English", "French"

    [MaxLength(100)]
    public string? Mood { get; set; }
    // e.g., "Melancholic", "Joyful", "Reflective"

    [MaxLength(200)]
    public string? CoverImagePath { get; set; }
    // e.g., "uploads/poems/moonlight.jpg"

    public bool IsPublic { get; set; } = true;
    // Determines if poem is visible to everyone

    public bool IsApproved { get; set; } = false;
    // Admin-only flag: must be approved to appear in anthology

    public bool IsFeatured { get; set; } = false;
    // Highlighted on homepage or special collections

    public int ViewCount { get; set; } = 0;
    // Tracks number of times poem was viewed

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    // When the poem was created

    public DateTime? UpdatedAt { get; set; }
    // Last edited time, if any

    public int UserId { get; set; }
    public User User { get; set; } = null!;
    // Reference to the author

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    // Optional: comments or reactions
}
