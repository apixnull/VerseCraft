using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VerseCraft.Areas.admin.ViewModels;
using VerseCraft.Data;
using VerseCraft.Models;

namespace VerseCraft.Areas.admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "Admin")] // 👮 Only accessible to Admins
    public class PoemsController : Controller
    {

        private readonly VerseCraftDbContext _context;

        public PoemsController(VerseCraftDbContext context)
        {
            _context = context;
        }

        // ================================================================
        // 📖 READ: Display all poems
        // GET: /Admin/Poem/DisplayPoem
        // ================================================================
        public IActionResult DisplayPoem()
        {
            var poems = _context.Poems
                .Select(p => new PoemFormViewModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    Genre = p.Genre,
                    IsPublic = p.IsPublic,
                    IsApproved = p.IsApproved,
                    IsFeatured = p.IsFeatured,
                    Author = p.Author
                })
                .ToList();

            return View(poems);
        }


        // ================================================================
        // ➕ CREATE: Show form to add a new poem
        // GET: /Admin/Poem/AddPoem
        // ================================================================
        public IActionResult AddPoem()
        {
            return View(new PoemFormViewModel());
        }

        // ================================================================
        // 💾 CREATE: Submit new poem
        // POST: /Admin/Poem/AddPoem
        // ================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPoem(PoemFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if UserId exists in the database
                if (model.UserId != null && !_context.Users.Any(u => u.Id == model.UserId))
                {
                    ModelState.AddModelError("UserId", "The specified user does not exist.");
                    return View(model);
                }

                // Check if AnthologyId exists in the database
                if (model.AnthologyId != null && !_context.Anthologies.Any(a => a.Id == model.AnthologyId))
                {
                    ModelState.AddModelError("AnthologyId", "The specified anthology does not exist.");
                    return View(model);
                }

                var poem = new Poem
                {
                    Title = model.Title,
                    Content = model.Content,
                    Summary = model.Summary,
                    Genre = model.Genre,
                    Style = model.Style,
                    Language = model.Language,
                    Mood = model.Mood,
                    IsPublic = model.IsPublic,
                    IsApproved = model.IsApproved,
                    IsFeatured = model.IsFeatured,
                    Author = model.Author ?? "NotFound", // Default if no author provided
                    UserId = model.UserId,  // Optional: only set if provided
                    AnthologyId = model.AnthologyId // Optional: only set if provided
                };

                // Handle Cover Image Upload
                if (model.CoverImageFile != null)
                {
                    // Generate a unique file name using GUID
                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.CoverImageFile.FileName);

                    // Create the directory if it doesn't exist
                    var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "poems");
                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    // Set the file path to save the image
                    var filePath = Path.Combine(uploadDir, uniqueFileName);

                    // Save the image file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.CoverImageFile.CopyToAsync(stream);
                    }

                    // Set the relative path for the image (to save in the database)
                    poem.CoverImagePath = $"/uploads/poems/{uniqueFileName}";   
                }

                // Add the poem to the database
                _context.Poems.Add(poem);
                await _context.SaveChangesAsync();

                TempData["SUCCESS"] = "Poem added successfully!";

                return RedirectToAction(nameof(DisplayPoem)); // Redirect to the list of poems
            }
            TempData["ERROR"] = "Failed to add the poem. Please try again.";
            return View(model);  // Return the model if validation fails
        }



        // ================================================================
        // 📝 UPDATE: Show edit form for existing poem
        // GET: /Admin/Poem/EditPoem/{id}
        // ================================================================
        [HttpGet]
        public IActionResult EditPoem(int id)
        {
            // Fetch the existing poem details
            var poem = _context.Poems
                .Where(p => p.Id == id)
                .Select(p => new PoemFormViewModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    Summary = p.Summary,
                    Genre = p.Genre,
                    Style = p.Style,
                    Language = p.Language,
                    Mood = p.Mood,
                    IsPublic = p.IsPublic,
                    IsApproved = p.IsApproved,
                    IsFeatured = p.IsFeatured,
                    Author = p.Author,
                    ExistingCoverImagePath = p.CoverImagePath, // Pass existing image path
                    UserId = p.UserId,
                    AnthologyId = p.AnthologyId
                })
                .FirstOrDefault();

            if (poem == null)
            {
                return NotFound();
            }

            return View(poem);
        }


        // ================================================================
        // 💾 UPDATE: Submit changes to poem
        // POST: /Admin/Poem/EditPoem/{id}
        // ================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPoem(int id, PoemFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                var poem = await _context.Poems.FindAsync(id);

                if (poem == null)
                {
                    return NotFound();
                }

                // Check if a new image is uploaded
                if (model.CoverImageFile != null)
                {
                    // Delete the existing cover image if a new one is uploaded
                    if (!string.IsNullOrEmpty(poem.CoverImagePath))
                    {
                        var existingFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", poem.CoverImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(existingFilePath))
                        {
                            System.IO.File.Delete(existingFilePath);
                        }
                    }

                    // Upload the new cover image
                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.CoverImageFile.FileName);
                    var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "poems");
                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    var filePath = Path.Combine(uploadDir, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.CoverImageFile.CopyToAsync(stream);
                    }

                    poem.CoverImagePath = $"/uploads/poems/{uniqueFileName}";
                }

                // Update the poem details
                poem.Title = model.Title;
                poem.Content = model.Content;
                poem.Summary = model.Summary;
                poem.Genre = model.Genre;
                poem.Style = model.Style;
                poem.Language = model.Language;
                poem.Mood = model.Mood;
                poem.IsPublic = model.IsPublic;
                poem.IsApproved = model.IsApproved;
                poem.IsFeatured = model.IsFeatured;
                poem.Author = model.Author ?? poem.Author; // Keep the existing author if not provided
                poem.UserId = model.UserId;
                poem.AnthologyId = model.AnthologyId;

                // Save changes to the database
                _context.Poems.Update(poem);
                await _context.SaveChangesAsync();

                TempData["SUCCESS"] = "Poem updated successfully!";
                return RedirectToAction(nameof(DisplayPoem)); // Redirect to the list of poems
            }

            TempData["ERROR"] = "Failed to update the poem. Please try again.";
            return View(model);  // Return the model if validation fails
        }

        // ================================================================
        // ❌ DELETE: Remove a poem
        // POST: /Admin/Poem/DeletePoem/{id}
        // ================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePoem(int id)
        {
            var poem = _context.Poems.FirstOrDefault(p => p.Id == id);
            if (poem == null)
            {
                TempData["ERROR"] = "Failed to delete the poem. Please try again.";
                return NotFound();
            }

            // Delete the cover image from wwwroot (if it exists)
            if (!string.IsNullOrEmpty(poem.CoverImagePath))
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", poem.CoverImagePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            // Remove the poem from the database
            _context.Poems.Remove(poem);
            _context.SaveChanges();

            TempData["SUCCESS"] = "Poem deleted successfully!";
            return RedirectToAction("DisplayPoem");
        }
     
    }
}
