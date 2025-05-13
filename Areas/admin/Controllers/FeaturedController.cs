using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VerseCraft.Data;
using VerseCraft.Models;

namespace VerseCraft.Areas.admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "Admin")] // 👈 Protects all actions in this controller
    public class FeaturedController : Controller
    {
        private readonly VerseCraftDbContext _context;

        public FeaturedController(VerseCraftDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> FeaturedItems()
        {
            var featuredItems = await _context.FeaturedItems
                .Include(f => f.Poem)
                .Include(f => f.Anthology)
                .ToListAsync();

            return View(featuredItems);
        }
        public async Task<IActionResult> SearchItemToAdd(string type)
        {
            if (type == "Poem")
            {
                var featuredPoemIds = await _context.FeaturedItems
                    .Where(f => f.WorkType == WorkType.Poem && f.PoemId != null)
                    .Select(f => f.PoemId!.Value)
                    .ToListAsync();

                var poems = await _context.Poems
                    .Where(p => !featuredPoemIds.Contains(p.Id) && p.IsPublic) // Added IsPublic check
                    .ToListAsync();

                ViewData["WorkType"] = "Poem";
                return View("SearchItemToAdd", poems);
            }
            else if (type == "Anthology")
            {
                var featuredAnthologyIds = await _context.FeaturedItems
                    .Where(f => f.WorkType == WorkType.Anthology && f.AnthologyId != null)
                    .Select(f => f.AnthologyId!.Value)
                    .ToListAsync();

                var anthologies = await _context.Anthologies
                    .Where(a => !featuredAnthologyIds.Contains(a.Id) && a.IsPublic) // Added IsPublic check
                    .ToListAsync();

                ViewData["WorkType"] = "Anthology";
                return View("SearchItemToAdd", anthologies);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> AddFeature(string type, int id)
        {
            if (type == "Poem")
            {
                bool alreadyFeatured = await _context.FeaturedItems
                    .AnyAsync(f => f.WorkType == WorkType.Poem && f.PoemId == id);

                if (alreadyFeatured)
                {
                    TempData["ERROR"] = "This poem is already featured.";
                    return RedirectToAction("SearchItemToAdd", new { type });
                }

                _context.FeaturedItems.Add(new FeaturedItem
                {
                    WorkType = WorkType.Poem,
                    PoemId = id,
                   
                });
            }
            else if (type == "Anthology")
            {
                bool alreadyFeatured = await _context.FeaturedItems
                    .AnyAsync(f => f.WorkType == WorkType.Anthology && f.AnthologyId == id);

                if (alreadyFeatured)
                {
                    TempData["Message"] = "This anthology is already featured.";
                    return RedirectToAction("SearchItemToAdd", new { type });
                }

                _context.FeaturedItems.Add(new FeaturedItem
                {
                    WorkType = WorkType.Anthology,
                    AnthologyId = id,
                   
                });
            }
            else
            {
                TempData["ERROR"] = "Invalid type.";
                return RedirectToAction("SearchItemToAdd", new { type });
            }

            await _context.SaveChangesAsync();
            TempData["SUCCESS"] = $"{type} successfully featured.";
            return RedirectToAction("SearchItemToAdd", new { type });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFeature(int id, string type)
        {
            var featuredItem = await _context.FeaturedItems
                .FirstOrDefaultAsync(f => f.Id == id);

            if (featuredItem == null)
            {
                TempData["ERROR"] = "Featured item not found.";
                return RedirectToAction("FeaturedItems");
            }

            if (type == "Poem")
            {
                _context.FeaturedItems.Remove(featuredItem);
            }
            else if (type == "Anthology")
            {
                _context.FeaturedItems.Remove(featuredItem);
            }

            await _context.SaveChangesAsync();

            TempData["SUCCESS"] = $"{type} successfully removed from featured.";
            return RedirectToAction("FeaturedItems");
        }

    }
}
