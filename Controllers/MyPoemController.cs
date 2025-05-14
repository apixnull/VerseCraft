using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using VerseCraft.Areas.admin.ViewModels;
using VerseCraft.Data;
using VerseCraft.Models;

namespace VerseCraft.Controllers
{


    public class MyPoemController : Controller
    {
        private readonly VerseCraftDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public MyPoemController(VerseCraftDbContext context, IWebHostEnvironment env)
        {
            _hostEnvironment = env;
            _context = context;

        }

        [HttpGet]
        public IActionResult CreatePoem(int? id)  // 'id' is the anthologyId from the route
        {
            var model = new PoemFormViewModel
            {
                AnthologyId = id // Add this property to your ViewModel
            };
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> CreatePoem(PoemFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ERROR"] = "Please fix the error in the form";
                return View(model);
            }

            // 1. Handle cover image upload
            string uniqueFileName = "";
            if (model.NewCoverImagePath != null)
            {
                uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(model.NewCoverImagePath.FileName)}";
                var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "poems");
                Directory.CreateDirectory(uploadsFolder);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await model.NewCoverImagePath.CopyToAsync(stream);
            }

            // 2. Map view model to Poem
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
                UserId = User.Identity!.IsAuthenticated
                    ? (int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ? (int?)userId : null)
                    : null,
                IsPublic = model.IsPublic,
                CreatedAt = DateTime.UtcNow
            };

            // 3. Save Poem
            _context.Poems.Add(poem);
            await _context.SaveChangesAsync();

            // 4. Link to anthology if provided
            if (model.AnthologyId.HasValue)
            {
                var anthologyPoem = new AnthologyPoem
                {
                    PoemId = poem.Id,
                    AnthologyId = model.AnthologyId.Value
                };

                _context.AnthologyPoems.Add(anthologyPoem);
                await _context.SaveChangesAsync();

                TempData["SUCCESS"] = "Poem added to anthology successfully!";
                return RedirectToAction("ViewAnthology", "MyAnthology", new { id = model.AnthologyId.Value });
            }

            TempData["SUCCESS"] = "Poem created successfully!";
            return RedirectToAction("DisplayCollections", "MyCollection");
        }



        // lets create the view 
        [HttpGet]
        public async Task<IActionResult> ViewPoem(int id)
        {
            var poem = await _context.Poems.FindAsync(id);
            if (poem == null)
            {
                return NotFound();
            }

            return View(poem);
        }

        // GET: load existing poem into the form
        [HttpGet]
        public async Task<IActionResult> EditPoem(int id)
        {
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
                IsPublic = poem.IsPublic,
                AnthologyId = poem.AnthologyPoems.FirstOrDefault()?.AnthologyId
            };

            return View(model);
        }

        // POST: apply edits, handle image replacement
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPoem(PoemFormViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var poem = await _context.Poems.FindAsync(model.Id);
            if (poem == null) return NotFound();

            // 1. Replace cover if a new file was uploaded
            if (model.NewCoverImagePath != null)
            {
                // delete old file
                if (!string.IsNullOrEmpty(poem.CoverImagePath))
                {
                    var oldFile = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "poems", poem.CoverImagePath);
                    if (System.IO.File.Exists(oldFile))
                        System.IO.File.Delete(oldFile);
                }

                // save new
                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(model.NewCoverImagePath.FileName)}";
                var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "poems");
                Directory.CreateDirectory(uploadsFolder);
                var newPath = Path.Combine(uploadsFolder, uniqueFileName);
                using var stream = new FileStream(newPath, FileMode.Create);
                await model.NewCoverImagePath.CopyToAsync(stream);
                poem.CoverImagePath = uniqueFileName;
            }

            // 2. Update fields
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
            poem.IsPublic = model.IsPublic;
            poem.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["SUCCESS"] = "Poem Updated successfully.";
            // 4. Link to anthology if provided
            if (model.AnthologyId.HasValue)
            {
                return RedirectToAction("ViewPoemInAntholoy", "MyAnthology", new { id = model.Id});
            }

            return RedirectToAction("ViewPoem", new { id = poem.Id });
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
            return RedirectToAction("DisplayCollections", "MyCollection");
        }

    }
}
