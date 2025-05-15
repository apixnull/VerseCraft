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
        // GET: /Admin/Publish
        public async Task<IActionResult> DisplayPublishItems()
        {
            // 1. Find all approved & public Poem IDs
            var approvedPublicPoemIds = await _context.Poems
                .Where(p => p.IsApproved && p.IsPublic)
                .Select(p => p.Id)
                .ToListAsync();

            // 2. Find all approved & public Anthology IDs
            var approvedPublicAnthologyIds = await _context.Anthologies
                .Where(a => a.IsApproved && a.IsPublic)
                .Select(a => a.Id)
                .ToListAsync();

            // 3. Add missing PublicItems for Poems
            var existingPoemPublics = await _context.PublicItems
                .Where(pi => pi.WorkType == WorkType.Poem)
                .Select(pi => pi.PoemId!.Value)
                .ToListAsync();

            var poemsToAdd = approvedPublicPoemIds
                .Except(existingPoemPublics)
                .Select(id => new PublicItem
                {
                    WorkType = WorkType.Poem,
                    PoemId = id
                });
            _context.PublicItems.AddRange(poemsToAdd);

            // 4. Add missing PublicItems for Anthologies
            var existingAnthologyPublics = await _context.PublicItems
                .Where(pi => pi.WorkType == WorkType.Anthology)
                .Select(pi => pi.AnthologyId!.Value)
                .ToListAsync();

            var anthologiesToAdd = approvedPublicAnthologyIds
                .Except(existingAnthologyPublics)
                .Select(id => new PublicItem
                {
                    WorkType = WorkType.Anthology,
                    AnthologyId = id
                });
            _context.PublicItems.AddRange(anthologiesToAdd);

            // 5. Remove any PublicItems whose ID is no longer in the approved+public lists
            var toRemove = await _context.PublicItems
                .Where(pi =>
                    (pi.WorkType == WorkType.Poem && !approvedPublicPoemIds.Contains(pi.PoemId!.Value))
                 || (pi.WorkType == WorkType.Anthology && !approvedPublicAnthologyIds.Contains(pi.AnthologyId!.Value))
                )
                .ToListAsync();

            _context.PublicItems.RemoveRange(toRemove);

            // 6. Persist all changes in one SaveChanges call
            await _context.SaveChangesAsync();

            // 7. Reload and return for display
            var allPublicItems = await _context.PublicItems
                .Include(pi => pi.Poem)
                .Include(pi => pi.Anthology)
                .ToListAsync();

            return View(allPublicItems);
        }


    }
}

