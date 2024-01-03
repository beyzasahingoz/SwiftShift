using Bitirme.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bitirme.Controllers
{
    public class MainController : Controller
    {
        private readonly ILogger<MainController> _logger;

        private readonly DbContextSwiftShift _context;

        private readonly UserManager<ApplicationUser> _userManager;
        public MainController(ILogger<MainController> logger, DbContextSwiftShift context, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Profile()
        {
            return View();
        }
        public IActionResult Explore()
        {
            return View();
        }
        public IActionResult Message()
        {
            return View();
        }
        public IActionResult Advert()
        {
            ViewData["Title"] = "İlan Ekle";
            return View();
        }
        public IActionResult Order()
        {
            return View();
        }
    }
}
