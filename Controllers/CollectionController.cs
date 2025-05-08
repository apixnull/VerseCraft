using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


using VerseCraft.Data;

namespace VerseCraft.Controllers
{
    [Authorize(Roles = "User,Admin")]
    public class CollectionController : Controller
    {
        private readonly VerseCraftDbContext _context;
     

        public CollectionController(VerseCraftDbContext context)
        {
            _context = context;
           
        }

        public IActionResult MainDashboard()
        {
            return View();
        }

     
    }
}
