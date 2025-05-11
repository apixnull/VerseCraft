using System.Security.Claims;
using DotNetEnv;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VerseCraft.Areas.admin.ViewModels;
using VerseCraft.Data;
using VerseCraft.Models;
using VerseCraft.ViewModels;

namespace VerseCraft.Controllers
{
    [Authorize(Roles = "User,Admin")]
    public class MyCollectionController : Controller
    {
        private readonly VerseCraftDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;


        public MyCollectionController(VerseCraftDbContext context, IWebHostEnvironment env)
        {
            _hostEnvironment = env;
            _context = context;
           
        }
        [HttpGet]
        public async Task<IActionResult> DisplayCollections()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized();
            }

            var user = await _context.Users
                .Include(u => u.Anthologies)
                    .ThenInclude(a => a.AnthologyPoems)
                        .ThenInclude(ap => ap.Poem)
                .Include(u => u.Poems)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound();

            var viewModel = new DisplayCollectionsViewModel
            {
                Anthologies = user.Anthologies
                    .OrderByDescending(a => a.UpdatedAt)
                    .ToList(),
                Poems = user.Poems
                    .OrderByDescending(p => p.CreatedAt)
                    .ToList()
            };

            return View(viewModel);
        }


    }
}
