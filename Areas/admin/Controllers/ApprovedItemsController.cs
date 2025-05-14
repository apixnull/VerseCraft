
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VerseCraft.Areas.admin.ViewModels;
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


        // Display items needing approval (both Poems and Anthologies)
        public async Task<IActionResult> PendingApprovals()
        {
            // Get IDs of all rejected items
            var rejectedPoemIds = await _context.Rejections
                .Where(r => r.ItemType == "Poem")
                .Select(r => r.ItemId)
                .ToListAsync();

            var rejectedAnthologyIds = await _context.Rejections
                .Where(r => r.ItemType == "Anthology")
                .Select(r => r.ItemId)
                .ToListAsync();

            var pendingItems = new PendingApprovalsViewModel
            {
                PendingPoems = await _context.Poems
                    .Where(p => !p.IsApproved &&
                                !rejectedPoemIds.Contains(p.Id) &&
                                p.IsPublic)
                    .ToListAsync(),

                PendingAnthologies = await _context.Anthologies
                    .Where(a => !a.IsApproved &&
                               !rejectedAnthologyIds.Contains(a.Id) &&
                               a.IsPublic)
                    .ToListAsync()
            };

            return View(pendingItems);
        }


        [HttpGet]
        public async Task<IActionResult> ReviewPoem(int id)
        {
            var poem = await _context.Poems
                .Include(p => p.User) // Include the User data
                .Include(p => p.AnthologyPoems) // Include the join table
                    .ThenInclude(ap => ap.Anthology) // Then include the Anthology for each join
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsApproved);

            if (poem == null)
                return NotFound();

            return View(poem);
        }

        [HttpGet]
        public async Task<IActionResult> ReviewAnthology(int id)
        {
            var anthology = await _context.Anthologies
                .Include(a => a.User) // Include the User data
                .Include(a => a.AnthologyPoems) // Include the join table
                    .ThenInclude(ap => ap.Poem) // Then include the Poem for each join
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsApproved);

            if (anthology == null)
                return NotFound();

            return View(anthology);
        }

        [HttpPost]
        public async Task<IActionResult> RejectPoem(int id, string rejectionReason)
        {
            var poem = await _context.Poems.FindAsync(id);

            if (poem == null || poem.IsApproved)
                return NotFound();

            poem.IsApproved = false; // Ensure it's marked as not approved (though it should already be false)

            var rejection = new Rejection
            {
                ItemId = id,
                ItemType = "Poem",
                RejectionReason = rejectionReason,
                RejectedAt = DateTime.UtcNow
            };

            _context.Rejections.Add(rejection);
            await _context.SaveChangesAsync();
            TempData["SUCCESS"] = "Poem rejected successfully";
            return RedirectToAction(nameof(PendingApprovals));
        }

        [HttpPost]
        public async Task<IActionResult> RejectAnthology(int id, string rejectionReason)
        {
            var anthology = await _context.Anthologies.FindAsync(id);

            if (anthology == null || anthology.IsApproved)
                return NotFound();

            anthology.IsApproved = false; // Ensure it's marked as not approved (though it should already be false)

            var rejection = new Rejection
            {
                ItemId = id,
                ItemType = "Anthology",
                RejectionReason = rejectionReason,
                RejectedAt = DateTime.UtcNow
            };

            _context.Rejections.Add(rejection);
            await _context.SaveChangesAsync();
            TempData["SUCCESS"] = "Anthology rejected successfully";
            return RedirectToAction(nameof(PendingApprovals));
        }
        [HttpPost]
        public async Task<IActionResult> ApprovePoem(int id)
        {
            var poem = await _context.Poems.FindAsync(id);

            if (poem == null || poem.IsApproved)
                return NotFound();

            // Mark poem as approved
            poem.IsApproved = true;
            poem.UpdatedAt = DateTime.UtcNow;

            // Add to approved items
            var approvedItem = new ApprovedItem
            {
                WorkType = WorkType.Poem,
                PoemId = id,
                AnthologyId = null
            };

            // Remove any existing rejection record
            var rejection = await _context.Rejections
                .FirstOrDefaultAsync(r => r.ItemId == id && r.ItemType == "Poem");

            if (rejection != null)
            {
                _context.Rejections.Remove(rejection);
            }

            _context.ApprovedItems.Add(approvedItem);
            await _context.SaveChangesAsync();
            TempData["SUCCESS"] = "Poem Approved successfully";
            return RedirectToAction(nameof(PendingApprovals));
        }

        [HttpPost]
        public async Task<IActionResult> ApproveAnthology(int id)
        {
            var anthology = await _context.Anthologies.FindAsync(id);

            if (anthology == null || anthology.IsApproved)
                return NotFound();

            // Mark anthology as approved
            anthology.IsApproved = true;
            anthology.UpdatedAt = DateTime.UtcNow;

            // Add to approved items
            var approvedItem = new ApprovedItem
            {
                WorkType = WorkType.Anthology,
                AnthologyId = id,
                PoemId = null
            };

            // Remove any existing rejection record
            var rejection = await _context.Rejections
                .FirstOrDefaultAsync(r => r.ItemId == id && r.ItemType == "Anthology");

            if (rejection != null)
            {
                _context.Rejections.Remove(rejection);
            }

            _context.ApprovedItems.Add(approvedItem);
            await _context.SaveChangesAsync();
            TempData["SUCCESS"] = "Anthology Approved successfully";
            return RedirectToAction(nameof(PendingApprovals));
        }

        public async Task<IActionResult> ApprovalHistory()
        {
            // First, get all existing poem and anthology IDs for efficient checking
            var existingPoemIds = await _context.Poems.Select(p => p.Id).ToListAsync();
            var existingAnthologyIds = await _context.Anthologies.Select(a => a.Id).ToListAsync();

            // Clean up orphaned approved items
            var orphanedApprovedItems = await _context.ApprovedItems
                .Where(a => (a.WorkType == WorkType.Poem && !existingPoemIds.Contains((int)a.PoemId!)) ||
                             (a.WorkType == WorkType.Anthology && !existingAnthologyIds.Contains((int)a.AnthologyId!)))
                .ToListAsync();

            if (orphanedApprovedItems.Any())
            {
                _context.ApprovedItems.RemoveRange(orphanedApprovedItems);
            }

            // Clean up orphaned rejections
            var orphanedRejections = await _context.Rejections
                .Where(r => (r.ItemType == "Poem" && !existingPoemIds.Contains(r.ItemId)) ||
                             (r.ItemType == "Anthology" && !existingAnthologyIds.Contains(r.ItemId)))
                .ToListAsync();

            if (orphanedRejections.Any())
            {
                _context.Rejections.RemoveRange(orphanedRejections);
            }

            // Save changes if we cleaned up anything
            if (orphanedApprovedItems.Any() || orphanedRejections.Any())
            {
                await _context.SaveChangesAsync();
            }

            // Now get the cleaned-up history
            var history = new ApprovalHistoryViewModel
            {
                ApprovedItems = await _context.ApprovedItems
                    .Include(a => a.Poem)
                        .ThenInclude(p => p.User)
                    .Include(a => a.Anthology)
                        .ThenInclude(a => a.User)
                    .OrderByDescending(a => a.Poem != null ? a.Poem.UpdatedAt : a.Anthology!.UpdatedAt)
                    .ToListAsync(),

                RejectedItems = await _context.Rejections
                    .OrderByDescending(r => r.RejectedAt)
                    .ToListAsync()
            };

            return View(history);
        }
    }
}