using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VerseCraft.Data;
using VerseCraft.Models;


namespace VerseCraft.Areas.admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "Admin")] // 👈 Protects all actions in this controller
    public class ApprovedItemsController : Controller
    {
        private readonly VerseCraftDbContext _context;

        public ApprovedItemsController(VerseCraftDbContext context)
        {
            _context = context;
        }

        // Display approved items (Poems and Anthologies)
        public async Task<IActionResult> DisplayApprovedItems()
        {
            // Query the approved items (Poems and Anthologies)
            var approvedItems = await _context.ApprovedItems
                .Include(ai => ai.Poem)
                .Include(ai => ai.Anthology)
                .ToListAsync();

            return View(approvedItems);
        }

        [HttpGet]
        public async Task<IActionResult> SearchItemToApprove(string type)
        {
            if (type == "Poem")
            {
                var approvedPoemIds = await _context.ApprovedItems
                    .Where(a => a.WorkType == WorkType.Poem && a.PoemId != null)
                    .Select(a => a.PoemId!.Value)
                    .ToListAsync();

                var poemsToApprove = await _context.Poems
                    .Where(p => !approvedPoemIds.Contains(p.Id))
                    .ToListAsync();

                ViewData["WorkType"] = "Poem";
                return View("SearchItemToApprove", poemsToApprove);
            }
            else if (type == "Anthology")
            {
                var approvedAnthologyIds = await _context.ApprovedItems
                    .Where(a => a.WorkType == WorkType.Anthology && a.AnthologyId != null)
                    .Select(a => a.AnthologyId!.Value)
                    .ToListAsync();

                var anthologiesToApprove = await _context.Anthologies
                    .Where(a => !approvedAnthologyIds.Contains(a.Id))
                    .ToListAsync();

                ViewData["WorkType"] = "Anthology";
                return View("SearchItemToApprove", anthologiesToApprove);
            }

            return BadRequest("Invalid type");
        }

        [HttpPost]
        public async Task<IActionResult> AddApprovedItem(string type, int id)
        {
            if (type == "Poem")
            {
                bool alreadyApproved = await _context.ApprovedItems.AnyAsync(a => a.WorkType == WorkType.Poem && a.PoemId == id);
                if (alreadyApproved)
                {
                    TempData["Error"] = "This poem is already approved.";
                    return RedirectToAction("SearchItemToApprove", new { type = "Poem" });
                }

                var poem = await _context.Poems.FindAsync(id);
                if (poem == null)
                    return NotFound();

                var approvedPoem = new ApprovedItem
                {
                    WorkType = WorkType.Poem,
                    PoemId = id
                };

                _context.ApprovedItems.Add(approvedPoem);
                await _context.SaveChangesAsync();

                return RedirectToAction("SearchItemToApprove", new { type = "Poem" });
            }
            else if (type == "Anthology")
            {
                bool alreadyApproved = await _context.ApprovedItems.AnyAsync(a => a.WorkType == WorkType.Anthology && a.AnthologyId == id);
                if (alreadyApproved)
                {
                    TempData["Error"] = "This anthology is already approved.";
                    return RedirectToAction("SearchItemToApprove", new { type = "Anthology" });
                }

                var anthology = await _context.Anthologies.FindAsync(id);
                if (anthology == null)
                    return NotFound();

                var approvedAnthology = new ApprovedItem
                {
                    WorkType = WorkType.Anthology,
                    AnthologyId = id
                };

                _context.ApprovedItems.Add(approvedAnthology);
                await _context.SaveChangesAsync();

                return RedirectToAction("SearchItemToApprove", new { type = "Anthology" });
            }

            return BadRequest("Invalid type specified.");
        }


        [HttpPost]
        public async Task<IActionResult> RemoveApprovedItem(int id)
        {
            var item = await _context.ApprovedItems.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            _context.ApprovedItems.Remove(item);
            await _context.SaveChangesAsync();

            return RedirectToAction("DisplayApprovedItems");
        }

    }
}
