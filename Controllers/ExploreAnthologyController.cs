using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VerseCraft.Data;
using VerseCraft.Models;

namespace VerseCraft.Controllers
{
    [Authorize(Roles = "User,Admin")]
    public class ExploreAnthologyController : Controller
    {
        private readonly VerseCraftDbContext _context;

        public ExploreAnthologyController(VerseCraftDbContext context)
        {
            _context = context;
        }

        // GET: /ExploreAnthology/DisplayAnthologies
        public async Task<IActionResult> DisplayAnthologies()
        {
            var publicApprovedAnthologies = await _context.Anthologies
                // 1. Only those marked public & approved
                .Where(a => a.IsPublic && a.IsApproved)
                // 2. Load the creator
                .Include(a => a.User)
                // 3. Load the anthology→poem join, then each poem
                .Include(a => a.AnthologyPoems)
                    .ThenInclude(ap => ap.Poem)
                // 4. Newest first
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return View(publicApprovedAnthologies);
        }

    }
}
