using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VerseCraft.Areas.admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "Admin")] // 👈 Protects all actions in this controller
    public class DashboardController : Controller
    {
        public IActionResult Main()
        {
            return View();
        }
    }
}
