using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using VerseCraft.Data;
using VerseCraft.Models;

namespace VerseCraft.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly VerseCraftDbContext _db;
        public DashboardController(VerseCraftDbContext db) => _db = db;

        public async Task<IActionResult> Main()
        {
            var vm = new DashboardViewModel
            {
                UsersCount = await _db.Users.CountAsync(),
                PoemsCount = await _db.Poems.CountAsync(),
                AnthologiesCount = await _db.Anthologies.CountAsync(),
                // Only count FeaturedItems where the PoemId or AnthologyId is non-null
                FeaturedCount = await _db.FeaturedItems
                                                    .CountAsync(f =>
                                                        (f.WorkType == WorkType.Poem && f.PoemId != null) ||
                                                        (f.WorkType == WorkType.Anthology && f.AnthologyId != null)
                                                    ),
                PublishedCount = await _db.PublicItems.CountAsync(),
                PendingPoemsCount = await _db.Poems.CountAsync(p => !p.IsApproved),
                PendingAnthologiesCount = await _db.Anthologies.CountAsync(a => !a.IsApproved),

                QuickLinks = new[]
                {
            new QuickLink("Manage Users",      "Users",      "DisplayUser"),
            new QuickLink("Manage Poems",      "Poems",      "DisplayPoems"),
            new QuickLink("Manage Anthologies","Anthology",  "DisplayAnthologies"),
            new QuickLink("Featured Items",    "Featured",   "FeaturedItems"),
            new QuickLink("Publish Items",     "Publish",    "DisplayPublishItems"),
            new QuickLink("Pending Approvals", "ApprovedItems", "PendingApprovals")
        }
            };

            return View(vm);
        }

    }

    public class DashboardViewModel
    {
        public int UsersCount { get; set; }
        public int PoemsCount { get; set; }
        public int AnthologiesCount { get; set; }
        public int FeaturedCount { get; set; }
        public int PublishedCount { get; set; }
        public int PendingPoemsCount { get; set; }
        public int PendingAnthologiesCount { get; set; }
        public QuickLink[] QuickLinks { get; set; }
    }

    public record QuickLink(string Title, string Controller, string Action);
}
