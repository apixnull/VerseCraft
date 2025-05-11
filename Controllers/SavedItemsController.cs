using Microsoft.AspNetCore.Mvc;

namespace VerseCraft.Controllers
{
    public class SavedItemsController : Controller
    {
        public IActionResult DisplaySavedItems()
        {
            return View();
        }
    }
}
