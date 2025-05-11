using VerseCraft.Models;

namespace VerseCraft.ViewModels
{
    public class DisplayCollectionsViewModel
    {
        public List<Anthology> Anthologies { get; set; } = new();
        public List<Poem> Poems { get; set; } = new();
    }
}
