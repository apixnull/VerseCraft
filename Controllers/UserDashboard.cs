using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VerseCraft.ViewModels;

using VerseCraft.Data;

namespace VerseCraft.Controllers
{
    [Authorize(Roles = "User,Admin")]
    public class UserDashboard : Controller
    {
        private readonly VerseCraftDbContext _context;
     

        public UserDashboard(VerseCraftDbContext context)
        {
            _context = context;
           
        }

        public IActionResult MainDashboard()
        {
            return View();
        }

        // GET: SubmitPoem
        [HttpGet]
        public IActionResult SubmitPoem()
        {
            return View(new PoemSubmissionViewModel() );
        }

        // POST: SubmitPoem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitPoem(PoemSubmissionViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

            string? imagePath = null;

            // Define a custom path outside WebRoot (relative to project directory)
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "poems");

            // Ensure the directory exists
            Directory.CreateDirectory(uploadsFolder);

            if (model.CoverImage != null && model.CoverImage.Length > 0)
            {
                // Generate a unique file name
                var fileName = Guid.NewGuid() + Path.GetExtension(model.CoverImage.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Save the file to the custom path
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.CoverImage.CopyToAsync(stream);
                }

                // Set the relative path to the image (for database use)
                imagePath = "/uploads/poems/" + fileName;
            }

            // Create a new poem record
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
                CreatedAt = DateTime.UtcNow,
                UserId = userId,
                CoverImagePath = imagePath,
                IsApproved = false, // Pending approval by default
                IsFeatured = false,
                ViewCount = 0
            };

            // Save poem in the database
            _context.Poems.Add(poem);
            await _context.SaveChangesAsync();

            TempData["SUCCESS"] = "Poem submitted successfully and is pending approval.";
            return RedirectToAction("MainDashboard");
        }
    }
}
