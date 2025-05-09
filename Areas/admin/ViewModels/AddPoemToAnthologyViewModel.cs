using VerseCraft.Models;

namespace VerseCraft.Areas.admin.ViewModels
{
    public class AddPoemToAnthologyViewModel
    {
        public int AnthologyId { get; set; }
        public string? SearchQuery { get; set; }
        public List<Poem> Poems { get; set; } = new();

    }
}
