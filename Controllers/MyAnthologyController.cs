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

            return View(poem);
        }
    }
}
