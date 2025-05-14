using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VerseCraft.Data;
using VerseCraft.Models;

namespace VerseCraft.Areas.admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "Admin")] // 👈 Protects all actions in this controller
    public class PublishController : Controller
    {
        private readonly VerseCraftDbContext _context;  

        public PublishController(VerseCraftDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Publish
        public async Task<IActionResult> DisplayPublishItems()
        {
            var publicItems = await _context.PublicItems
                .Include(pi => pi.Poem)
                .Include(pi => pi.Anthology)
                .Where(pi =>
                    // Poem items: must exist, be public, and approved
                    (pi.WorkType == WorkType.Poem
                     && pi.Poem != null
                     && pi.Poem.IsPublic
                     && pi.Poem.IsApproved)
                    ||
                    // Anthology items: must exist, be public, and approved
                    (pi.WorkType == WorkType.Anthology
                     && pi.Anthology != null
                     && pi.Anthology.IsPublic
                     && pi.Anthology.IsApproved)
                )
                .ToListAsync();

            return View(publicItems);
        }

    }
}

