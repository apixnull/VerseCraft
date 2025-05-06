using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VerseCraft.Areas.Admin.ViewModels;
using VerseCraft.Data;
using VerseCraft.Models;

namespace VerseCraft.Areas.admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "Admin")] // 👈 Protects all actions in this controller
    public class AnthologyController : Controller
    {
        private readonly VerseCraftDbContext _context;

        public AnthologyController(VerseCraftDbContext context)
        {
            _context = context;
        }

        /******************************************** DISPLAY ANTHOLOGOY ********************************************/

        public IActionResult DisplayAnthology()
        {
            // Query the anthology content from the database
            var anthologyList = _context.Anthologies.ToList();

            // Pass the anthology data to the view
            return View(anthologyList);
        }

        /******************************************** ADD ANTHOLOGOY ********************************************/

        // GET: Add Anthology
        public IActionResult AddAnthology()
        {
            return View();
        }

        // POST: AddAnthology (Form submission)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAnthology(AnthologyViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Generate a unique filename using GUID
                string imagePath = string.Empty;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    // Generate a GUID for the file name to ensure uniqueness
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);

                    // Define the file path where the image will be stored
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "anthologies", fileName);

                    // Ensure the directory exists
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                    // Save the image to the defined path
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }

                    // Set the relative path to the image (for use in the database)
                    imagePath = "/uploads/anthologies/" + fileName;
                }

                // Create a new Anthology object
                var anthology = new Anthology
                {
                    Title = model.Title,
                    Content = model.Content,
                    Author = model.Author,
                    CopyrightNotice = model.CopyrightNotice ?? "",
                    LicenseType = model.LicenseType ?? "",
                    TermsAndConditions = model.TermsAndConditions ?? "",
                    ImagePath = imagePath ?? ""
                };

                // Save the anthology in the database
                _context.Anthologies.Add(anthology);
                await _context.SaveChangesAsync();

                TempData["SUCCESS"] = "Anthology added successfully!";
                // Redirect to the DisplayAnthology view
                return RedirectToAction("DisplayAnthology");
            }

            // If the model is invalid, return to the AddAnthology view with validation errors
            return View(model);
        }

        /******************************************** DELETE ANTHOLOGOY ********************************************/

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAnthology(int id)
        {
            var anthology = await _context.Anthologies.FindAsync(id);

            if (anthology == null)
            {
                return NotFound();
            }

            // Restrict deletion to images inside uploads/anthologies only
            if (!string.IsNullOrWhiteSpace(anthology.ImagePath) && anthology.ImagePath.StartsWith("/uploads/anthologies/"))
            {
                var fileName = Path.GetFileName(anthology.ImagePath);
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "anthologies", fileName);

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }

            _context.Anthologies.Remove(anthology);
            await _context.SaveChangesAsync();

            TempData["SUCCESS"] = "Anthology deleted successfully!";
            return RedirectToAction("DisplayAnthology");
        }




        /******************************************** UPDATE ANTHOLOGOY ********************************************/
        // GET: UpdateAnthology
        public async Task<IActionResult> UpdateAnthology(int id)
        {
            var anthology = await _context.Anthologies.FindAsync(id);
            if (anthology == null)
            {
                return NotFound();
            }

            var viewModel = new AnthologyViewModel
            {
                Title = anthology.Title,
                Content = anthology.Content,
                Author = anthology.Author,
                CopyrightNotice = anthology.CopyrightNotice,
                LicenseType = anthology.LicenseType,
                TermsAndConditions = anthology.TermsAndConditions,
                ExistingImagePath = anthology.ImagePath
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAnthology(int id, AnthologyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var anthology = await _context.Anthologies.FindAsync(id);

            if (anthology == null)
            {
                return NotFound();
            }

            // Update basic fields
            anthology.Title = model.Title;
            anthology.Content = model.Content;
            anthology.Author = model.Author;
            anthology.CopyrightNotice = model.CopyrightNotice ?? "";
            anthology.LicenseType = model.LicenseType ?? "";
            anthology.TermsAndConditions = model.TermsAndConditions ?? "";

            // Handle image update
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                // Delete old image
                if (!string.IsNullOrWhiteSpace(anthology.ImagePath))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", anthology.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Save new image
                var fileName = Guid.NewGuid() + Path.GetExtension(model.ImageFile.FileName);
                var newImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/anthologies", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(newImagePath)!);

                using (var stream = new FileStream(newImagePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                anthology.ImagePath = "/uploads/anthologies/" + fileName;
            }

            _context.Anthologies.Update(anthology);
            await _context.SaveChangesAsync();

            TempData["SUCCESS"] = "Anthology updated successfully!";
            return RedirectToAction("DisplayAnthology");
        }



    }
}
