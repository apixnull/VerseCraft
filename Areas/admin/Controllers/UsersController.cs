using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VerseCraft.Data;

namespace VerseCraft.Areas.admin.Controllers
{

    [Area("admin")]
    [Authorize(Roles = "Admin")] // 👈 Protects all actions in this controller
    public class UsersController : Controller
    {

        private readonly VerseCraftDbContext _context;

        public UsersController(VerseCraftDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> DisplayUser()
        {
            var users = await _context.Users.ToListAsync(); // 👈 Return full user list
            return View(users); // 👈 Pass as model
        }

    }
}
