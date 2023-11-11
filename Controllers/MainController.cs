using Microsoft.AspNetCore.Mvc;

namespace Bitirme.Controllers
{
    public class MainController : Controller
    {
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
            return View();
        }
        public IActionResult Order()
        {
            return View();
        }
    }
}
