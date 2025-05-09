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
    public class PoemsController : Controller
    {

        private readonly VerseCraftDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public PoemsController(VerseCraftDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment; // 👈 For file uploads
            _context = context; 
        }

        /* ===========================      Display Poem        =========================== */       
        public IActionResult DisplayPoems()
        {
            var poems = _context.Poems.Select(p => new Poem
            {
                Id = p.Id,
                Title = p.Title
            }).ToList();

            ViewBag.TotalPoems = poems.Count;

            return View(poems);
        }
        /* ===========================      Create Poem        =========================== */
        [HttpGet]
        public IActionResult CreatePoem()
        {
            return View(new PoemFormViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreatePoem(PoemFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ERROR"] = "Please fix the error in the form";
                return View(model); 
            }

            // 1. Handle the new cover image upload
            string uniqueFileName = "";
            if (model.NewCoverImagePath != null)
            {
                // generate GUID filename
                uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(model.NewCoverImagePath.FileName)}";

                // ensure uploads/poems folder exists
                var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "poems");
                Directory.CreateDirectory(uploadsFolder);

                // save file
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await model.NewCoverImagePath.CopyToAsync(stream);
            }

            // 2. Map ViewModel ➔ Poem entity
            var poem = new Poem
            {
                Title = model.Title,
                Content = model.Content,
                Summary = model.Summary,
                Genre = model.Genre,
                Style = model.Style,
                Theme = model.Theme,
                Tags = model.Tags,
                Language = model.Language,
                Mood = model.Mood,
                LicenseType = model.LicenseType,
                CopyrightNotice = model.CopyrightNotice,
                CoverImagePath = uniqueFileName,
                AuthorName = model.AuthorName,
                UserId = model.UserId,
                CreatedAt = DateTime.UtcNow
            };

            // 3. Persist and redirect
            _context.Poems.Add(poem);
            await _context.SaveChangesAsync();
            TempData["SUCCESS"] = "User Added successfully";
            return RedirectToAction(nameof(DisplayPoems));

         
        }

        /* ===========================      Edit Poem        =========================== */
        /* ===========================      Edit Poem        =========================== */
        [HttpGet]
        public async Task<IActionResult> EditPoem(int? id)
        {
            if (id == null) return NotFound();

            var poem = await _context.Poems.FindAsync(id);
            if (poem == null) return NotFound();

            var model = new PoemFormViewModel
            {
                Id = poem.Id,
                Title = poem.Title,
                Content = poem.Content,
                Summary = poem.Summary,
                Genre = poem.Genre,
                Style = poem.Style,
                Theme = poem.Theme,
                Tags = poem.Tags,
                Language = poem.Language,
                Mood = poem.Mood,
                LicenseType = poem.LicenseType,
                CopyrightNotice = poem.CopyrightNotice,
                CoverImagePath = poem.CoverImagePath,
                AuthorName = poem.AuthorName,
                UserId = poem.UserId,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPoem(PoemFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.Id == null)
                return BadRequest();

            var poem = await _context.Poems.FindAsync(model.Id.Value);
            if (poem == null)
                return NotFound();

            // Handle new cover upload
            if (model.NewCoverImagePath != null)
            {
                // Delete old file
                if (!string.IsNullOrEmpty(poem.CoverImagePath))
                {
                    var oldPath = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "poems", poem.CoverImagePath);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                // Save new file
                var uniqueName = $"{Guid.NewGuid()}{Path.GetExtension(model.NewCoverImagePath.FileName)}";
                var uploadFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "poems");
                Directory.CreateDirectory(uploadFolder);
                var filePath = Path.Combine(uploadFolder, uniqueName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await model.NewCoverImagePath.CopyToAsync(stream);

                poem.CoverImagePath = uniqueName;
            }

            // Update properties
            poem.Title = model.Title;
            poem.Content = model.Content;
            poem.Summary = model.Summary;
            poem.Genre = model.Genre;
            poem.Style = model.Style;
            poem.Theme = model.Theme;
            poem.Tags = model.Tags;
            poem.Language = model.Language;
            poem.Mood = model.Mood;
            poem.LicenseType = model.LicenseType;
            poem.CopyrightNotice = model.CopyrightNotice;
            poem.AuthorName = model.AuthorName;
            poem.UserId = model.UserId;
            poem.UpdatedAt = DateTime.UtcNow;

            _context.Poems.Update(poem);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(DisplayPoems));
        }

        /* ===========================      Delete Poem        =========================== */
        [HttpPost]
        public async Task<IActionResult> DeletePoem(int id)
        {
            // 1. Find the poem
            var poem = await _context.Poems.FindAsync(id);
            if (poem == null)
            {
                TempData["ERROR"] = "Poem not found";   
                return View();
            }

            // 2. Delete the cover image file if it exists
            if (!string.IsNullOrEmpty(poem.CoverImagePath))
            {
                var filePath = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "poems", poem.CoverImagePath);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            // 3. Remove from database
            _context.Poems.Remove(poem);
            await _context.SaveChangesAsync();
            TempData["SUCCESS"] = "Successfully delete the poem";

            // 4. Redirect back to the list
            return RedirectToAction(nameof(DisplayPoems));
        }

        /* ===========================      Display Specific Poem      =========================== */
        [HttpGet]
        public async Task<IActionResult> DisplaySpecificPoem(int id)
        {
            var poem = await _context.Poems
                                     .FirstOrDefaultAsync(p => p.Id == id);

            if (poem == null)
            {
                TempData["ERROR"] = "Poem not found.";
                return RedirectToAction(nameof(DisplayPoems));
            }

            return View(poem);
        }

    }
}
