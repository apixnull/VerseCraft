using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VerseCraft.Data;

namespace VerseCraft.Controllers
{
    [Authorize(Roles = "User,Admin")]
    public class ExplorePoemController : Controller
    {
        private readonly VerseCraftDbContext _db;
        public ExplorePoemController(VerseCraftDbContext db) => _db = db;

        public async Task<IActionResult> DisplayPoems()
        {
            // fetch only approved & published poems, newest first
            var poems = await _db.Poems
                .Where(p => p.IsApproved && p.IsPublic)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(poems);
        }
    }
}
