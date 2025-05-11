using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using VerseCraft.Data;
using VerseCraft.Models;
using VerseCraft.Areas.admin.ViewModels;

using Microsoft.EntityFrameworkCore;

namespace VerseCraft.Areas.admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "Admin")] // 👈 Protects all actions in this controller
    public class AnthologyController : Controller
    {
        private readonly VerseCraftDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AnthologyController(VerseCraftDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment; // 👈 For file uploads      
            _context = context;
        }

        /* ===========================      Display Anthologies       =========================== */
        public IActionResult DisplayAnthologies()
        {
            var anthologies = _context.Anthologies.Select(a => new Anthology
            {
                Id = a.Id,
                Title = a.Title,
            }).ToList();

            return View(anthologies);
        }

        /* ===========================      Create Anthologies       =========================== */
        public IActionResult CreateAnthology()
        {
            return View(new AnthologyFormViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateAnthology(AnthologyFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = "";

                // 1. Handle the new cover image upload
                if (model.CoverImage != null)
                {
                    // Generate a unique file name for the image
                    uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.CoverImage.FileName);

                    // Define the path to the uploads folder (anthologies folder)
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "anthologies");

                    // Ensure the folder exists
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Define the full file path to save the image
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save the file
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.CoverImage.CopyToAsync(fileStream);
                    }
                }

                // 2. Create the new anthology entry in the database
                var anthology = new Anthology
                {
                    Title = model.Title,
                    Description = model.Description,
                    AuthorName = model.AuthorName,
                    LicenseType = model.LicenseType,
                    CopyrightNotice = model.CopyrightNotice,
                    ImagePath = uniqueFileName, // Save the file name (path relative to wwwroot/uploads/anthologies)
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    UserId = model.UserId, // Optional, based on logged-in user
                    IsPublic = model.IsPublic
                };

                // 3. Add the anthology to the database and save changes
                _context.Anthologies.Add(anthology);
                await _context.SaveChangesAsync();

                // Redirect to the DisplayAnthologies page after creation
                return RedirectToAction(nameof(DisplayAnthologies));
            }

            // If the model state is invalid, return the view with validation messages
            return View(model);
        }

        /* ===========================      Delete Anthologies       =========================== */
        [HttpPost]
        [ActionName("DeleteAnthology")]
        public async Task<IActionResult> DeleteAnthology(int id)
        {
            // 1. Find the anthology by its ID
            var anthology = await _context.Anthologies
                                          .Where(a => a.Id == id)
                                          .FirstOrDefaultAsync();

            if (anthology == null)
            {
                // If not found, return a NotFound status
                return NotFound();
            }

            // 2. Handle the deletion of the image file from the server
            if (!string.IsNullOrEmpty(anthology.ImagePath))
            {
                string imagePath = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "anthologies", anthology.ImagePath);

                if (System.IO.File.Exists(imagePath))
                {
                    // If the file exists, delete it
                    System.IO.File.Delete(imagePath);
                }
            }

            // 3. Delete the anthology from the database
            _context.Anthologies.Remove(anthology);
            await _context.SaveChangesAsync();

            // 4. Redirect to the list of anthologies
            return RedirectToAction(nameof(DisplayAnthologies));
        }

        /* ===========================      Edit Anthologies       =========================== */
      
        public async Task<IActionResult> EditAnthology(int id)
        {
            var anthology = await _context.Anthologies.FindAsync(id);
            if (anthology == null) return NotFound();

            var viewModel = new AnthologyFormViewModel
            {
                Id = anthology.Id,
                Title = anthology.Title,
                Description = anthology.Description,
                AuthorName = anthology.AuthorName,
                LicenseType = anthology.LicenseType,
                CopyrightNotice = anthology.CopyrightNotice,
                ExistingImagePath = anthology.ImagePath,
                UserId = anthology.UserId,
                IsPublic = anthology.IsPublic
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditAnthology(AnthologyFormViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var anthology = await _context.Anthologies.FindAsync(model.Id);
            if (anthology == null) return NotFound();

            // Update fields
            anthology.Title = model.Title;
            anthology.Description = model.Description;
            anthology.AuthorName = model.AuthorName;
            anthology.LicenseType = model.LicenseType;
            anthology.CopyrightNotice = model.CopyrightNotice;
            anthology.UpdatedAt = DateTime.UtcNow;
            anthology.IsPublic = model.IsPublic;

            // ================== Image Handling ==================
            if (model.CoverImage != null)
            {
                // Delete old image
                if (!string.IsNullOrEmpty(model.ExistingImagePath))
                {
                    var oldPath = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "anthologies", model.ExistingImagePath);
                    if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                }

                // Save new image
                var newFileName = $"{Guid.NewGuid()}{Path.GetExtension(model.CoverImage.FileName)}";
                var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "anthologies");
                Directory.CreateDirectory(uploadsFolder); // Ensure folder exists
                var newPath = Path.Combine(uploadsFolder, newFileName);

                using var stream = new FileStream(newPath, FileMode.Create);
                await model.CoverImage.CopyToAsync(stream);

                anthology.ImagePath = newFileName;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DisplayAnthologies));
        }

        /* ===========================      Add Poem to Anthology =========================== */
        /* =========================== Add Poem to Anthology (GET) =========================== */
        [HttpGet]
        public IActionResult SearchPoem(int anthologyId)
        {
            // Get IDs of poems already added to the anthology
            var addedPoemIds = _context.AnthologyPoems
                                        .Where(ap => ap.AnthologyId == anthologyId)
                                        .Select(ap => ap.PoemId)
                                        .ToList();

            // Retrieve poems that are NOT in the anthology
            var poems = _context.Poems
                                .Where(p => !addedPoemIds.Contains(p.Id))
                                .ToList();

            var model = new AddPoemToAnthologyViewModel
            {
                AnthologyId = anthologyId,
                Poems = poems
            };

            return View(model);
        }


        /* =========================== Add Poem to Anthology (POST) =========================== */
        [HttpPost]
        public async Task<IActionResult> AddPoemToAnthology(int anthologyId, int poemId)
        {
            // Find the anthology and poem
            var anthology = await _context.Anthologies.FindAsync(anthologyId);
            var poem = await _context.Poems.FindAsync(poemId);

            if (anthology == null || poem == null)
            {
                return NotFound();
            }

            // Check if the relationship exists in the join table
            bool alreadyExists = _context.AnthologyPoems.Any(ap => ap.AnthologyId == anthologyId && ap.PoemId == poemId);

            if (!alreadyExists)
            {
                // Add new entry to AnthologyPoem join table
                _context.AnthologyPoems.Add(new AnthologyPoem
                {
                    AnthologyId = anthologyId,
                    PoemId = poemId
                });
                await _context.SaveChangesAsync();
            }

            TempData["SUCCESS"] = "Added Successfully";
            return RedirectToAction("SearchPoem", new { anthologyId });
        }

        /* =========================== View Specific Anthology =========================== */

        [HttpGet]
        public async Task<IActionResult> ViewAnthology(int id)
        {
            var anthology = await _context.Anthologies
                .Include(a => a.AnthologyPoems)
                    .ThenInclude(ap => ap.Poem)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (anthology == null)
            {
                return NotFound();
            }

            return View(anthology);
        }





      
    }
}
