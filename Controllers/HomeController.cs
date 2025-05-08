using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VerseCraft.Data;
using VerseCraft.Models;
using VerseCraft.Services;
using VerseCraft.ViewModels;

namespace VerseCraft.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly VerseCraftDbContext _context;
        private readonly EmailService _emailService;

        public HomeController(ILogger<HomeController> logger, VerseCraftDbContext context, EmailService emailService)
        {
            _context = context;
            _logger = logger;
            _emailService = emailService;
        }

        /***************************************      Index      ********************************************/ 
        public IActionResult Index()
        {
            return View();
        }


        /***************************************      About Us    ********************************************/

        [HttpGet]
        public IActionResult AboutUs()
        {
            return View();
        }


        /***************************************      Contact Us      ********************************************/
        [HttpGet]
        public IActionResult ContactUs()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> ContactUs(ContactFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _emailService.SendContactMessageAsync(model.Name, model.Email, model.Content);
                TempData["SUCCESS"] = "Your message has been sent. Thank you for contacting us!";
                return RedirectToAction(nameof(AboutUs));
            }
            catch 
            {
                TempData["ERROR"] = "Failed to send! Try again Letter";
                // Optionally log the exception
                return View(model);
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
