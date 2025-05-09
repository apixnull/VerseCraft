namespace VerseCraft.Models
{
    public class AnthologyPoem
    {
        public int AnthologyId { get; set; }
        public Anthology Anthology { get; set; } = null!;

        public int PoemId { get; set; }
        public Poem Poem { get; set; } =null!;
    }
}
