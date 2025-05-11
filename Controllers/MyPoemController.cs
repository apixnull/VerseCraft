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
    }
}
