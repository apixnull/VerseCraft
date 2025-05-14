using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using VerseCraft.Areas.admin.ViewModels;
using VerseCraft.Data;
using VerseCraft.Models;

namespace VerseCraft.Controllers
{
    [Authorize(Roles = "User,Admin")]
    public class MyAnthologyController : Controller
    {
        private readonly VerseCraftDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public MyAnthologyController(VerseCraftDbContext context, IWebHostEnvironment env)
        {
            _hostEnvironment = env;
            _context = context;

        }


        [HttpGet]
        public IActionResult CreateAnthology()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAnthology(AnthologyFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = "";

                // 1. Handle the new cover image upload
                if (model.CoverImage != null)
                {
                    uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.CoverImage.FileName);

                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "anthologies");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.CoverImage.CopyToAsync(fileStream);
                    }
                }

                // 2. Get logged-in user ID from claims
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    model.UserId = userId;
                }

                // 3. Create and save anthology
                var anthology = new Anthology
                {
                    Title = model.Title,
                    Description = model.Description,
                    AuthorName = model.AuthorName,
                    LicenseType = model.LicenseType,
                    CopyrightNotice = model.CopyrightNotice,
                    ImagePath = uniqueFileName,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    UserId = model.UserId,
                    IsPublic = model.IsPublic
                };

                _context.Anthologies.Add(anthology);
                await _context.SaveChangesAsync();

                TempData["SUCCESS"] = "Anthology created successfully.";
                return RedirectToAction("DisplayCollections", "MyCollection");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ViewAnthology(int id)
        {
            // Fetch the anthology including the associated poems
            var anthology = _context.Anthologies
                .Where(a => a.Id == id)
                .Include(a => a.AnthologyPoems) // Include the related AnthologyPoems
                .ThenInclude(ap => ap.Poem)     // Include the associated Poem for each AnthologyPoem
                .FirstOrDefault();

            if (anthology == null)
            {
                TempData["ERROR"] = "Anthology not Found";
                return RedirectToAction("Index", "Home"); // Redirect to a different page if not found

            }

            TempData["CurrentAnthologyId"] = id;

            return View(anthology); // Pass the anthology and associated poems to the view
        }

        // lets create the view 
        [HttpGet]
        public async Task<IActionResult> ViewPoemInAntholoy(int id)
        {
            var poem = await _context.Poems.FindAsync(id);
            if (poem == null)
            {
                return NotFound();
            }

            TempData.Keep("CurrentAnthologyId");

            return View(poem);
        }

        // GET: MyAnthology/EditAnthology/5
        [HttpGet]
        public async Task<IActionResult> EditAnthology(int id)
        {
            var anthology = await _context.Anthologies.FindAsync(id);
            if (anthology == null)
                return NotFound();

            var vm = new AnthologyFormViewModel
            {
                Id = anthology.Id,
                Title = anthology.Title,
                Description = anthology.Description,
                ExistingImagePath = anthology.ImagePath,
                AuthorName = anthology.AuthorName,
                LicenseType = anthology.LicenseType,
                CopyrightNotice = anthology.CopyrightNotice,
                IsPublic = anthology.IsPublic
            };
            return View(vm);
        }

        // POST: MyAnthology/EditAnthology
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAnthology(AnthologyFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var anthology = await _context.Anthologies.FindAsync(model.Id);
            if (anthology == null)
                return NotFound();

            // Handle new image
            if (model.CoverImage != null)
            {
                // delete old file
                if (!string.IsNullOrEmpty(anthology.ImagePath))
                {
                    var oldPath = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "anthologies", anthology.ImagePath);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                // save new file
                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(model.CoverImage.FileName)}";
                var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "anthologies");
                Directory.CreateDirectory(uploadsFolder);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await model.CoverImage.CopyToAsync(stream);
                anthology.ImagePath = uniqueFileName;
            }

            // Update other fields
            anthology.Title = model.Title;
            anthology.Description = model.Description;
            anthology.AuthorName = model.AuthorName;
            anthology.LicenseType = model.LicenseType;
            anthology.CopyrightNotice = model.CopyrightNotice;
            anthology.IsPublic = model.IsPublic;
            anthology.UpdatedAt = DateTime.UtcNow;

            _context.Anthologies.Update(anthology);
            await _context.SaveChangesAsync();

            TempData["SUCCESS"] = "Anthology updated successfully.";
            return RedirectToAction("ViewAnthology", new { id = model.Id});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAnthology(int id)
        {
            var anthology = await _context.Anthologies.FindAsync(id);
            if (anthology == null) return NotFound();

            // 1. Delete the cover‐image file if it exists
            if (!string.IsNullOrEmpty(anthology.ImagePath))
            {
                var file = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "anthologies", anthology.ImagePath);
                if (System.IO.File.Exists(file))
                    System.IO.File.Delete(file);
            }

            // 2. Remove the anthology record
            _context.Anthologies.Remove(anthology);
            await _context.SaveChangesAsync();

            TempData["SUCCESS"] = "Anthology deleted successfully.";
            return RedirectToAction("DisplayCollections", "MyCollection");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePoem(int id)
        {

            var poem = await _context.Poems.FindAsync(id);
            if (poem == null) return NotFound();

            // If there’s a cover file, delete it
            if (!string.IsNullOrEmpty(poem.CoverImagePath))
            {
                var fullPath = Path.Combine(_hostEnvironment.WebRootPath, poem.CoverImagePath);
                if (System.IO.File.Exists(fullPath))
                    System.IO.File.Delete(fullPath);
            }

            _context.Poems.Remove(poem);
            await _context.SaveChangesAsync();
            TempData["SUCCESS"] = "Poem deleted successfully.";

            // Retrieve anthology ID from TempData
            if (TempData["CurrentAnthologyId"] != null && int.TryParse(TempData["CurrentAnthologyId"]!.ToString(), out int anthologyId))
            {
                return RedirectToAction("ViewAnthology", new { id = anthologyId });
            }

            return RedirectToAction("ViewAnthology"); // i want after deleting the poem to redirect to view anthology but the problem is view anthology will need an id to be passed
        }


        // CREATE THE AddExistingPoem here , in this part we will have a search functionality, seach funtionality will be server side,
        // by default we will display at least 10 poems that is not associated with this anthology , take note only display poem that is own by this user
        // in the view you will display each poem like a card ( i will send it to you later),
        /*
         * 
         */

        // first step first, lets create the get method for this . 
        [HttpGet]
        public async Task<IActionResult> AddExistingPoem(int anthologyId, string? search)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized();
            }

            var associatedPoemIds = await _context.AnthologyPoems
                .Where(ap => ap.AnthologyId == anthologyId)
                .Select(ap => ap.PoemId)
                .ToListAsync();

            var poemsQuery = _context.Poems
                .Where(p => p.UserId == userId && !associatedPoemIds.Contains(p.Id));

            if (!string.IsNullOrWhiteSpace(search))
            {
                poemsQuery = poemsQuery.Where(p => p.Title.Contains(search) || p.Tags!.Contains(search));
            }

            var poems = await poemsQuery
                .OrderByDescending(p => p.CreatedAt)
                .Take(10)
                .ToListAsync();

            ViewBag.AnthologyId = anthologyId;
            ViewBag.SearchTerm = search;

            return View(poems);
        }

        [HttpGet]
        public async Task<IActionResult> FindExistingPoem(int anthologyId)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized();
            }

            var associatedPoemIds = await _context.AnthologyPoems
                .Where(ap => ap.AnthologyId == anthologyId)
                .Select(ap => ap.PoemId)
                .ToListAsync();

            var poems = await _context.Poems
                .Where(p => p.UserId == userId && !associatedPoemIds.Contains(p.Id))
                .OrderByDescending(p => p.CreatedAt)
                .Take(10)
                .ToListAsync();

            ViewBag.AnthologyId = anthologyId;
            return View(poems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddExistingPoem(int anthologyId, int poemId)
        {
            // ensure both items exist
            var anthology = await _context.Anthologies.FindAsync(anthologyId);
            var poem = await _context.Poems.FindAsync(poemId);
            if (anthology == null || poem == null)
                return NotFound();

            // avoid duplicates
            bool already = await _context.AnthologyPoems
                .AnyAsync(ap => ap.AnthologyId == anthologyId && ap.PoemId == poemId);
            if (!already)
            {
                _context.AnthologyPoems.Add(new AnthologyPoem
                {
                    AnthologyId = anthologyId,
                    PoemId = poemId
                });
                await _context.SaveChangesAsync();
            }

            TempData["SUCCESS"] = "Poem added to anthology!";
            return RedirectToAction("ViewAnthology", new { id = anthologyId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePoem(int id /*=poemId*/, int anthologyId)
        {
            // 1) Remove the AnthologyPoem join
            var ap = await _context.AnthologyPoems
                                  .FirstOrDefaultAsync(x => x.PoemId == id && x.AnthologyId == anthologyId);
            if (ap != null)
            {
                _context.AnthologyPoems.Remove(ap);
                await _context.SaveChangesAsync();
            }

            TempData["SUCCESS"] = "Poem removed from anthology.";

            return RedirectToAction("ViewAnthology", new { id = anthologyId });
        }

    }
}
