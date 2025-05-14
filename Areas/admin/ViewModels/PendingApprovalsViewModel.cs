using VerseCraft.Models;

namespace VerseCraft.Areas.admin.ViewModels
{
    public class PendingApprovalsViewModel
    {
        public List<Poem> PendingPoems { get; set; } = new();
        public List<Anthology> PendingAnthologies { get; set; } = new();
    }
}
