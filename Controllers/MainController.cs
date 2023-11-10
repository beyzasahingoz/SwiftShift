using Microsoft.AspNetCore.Mvc;

namespace Bitirme.Controllers
{
    public class MainController : Controller
    {
        public IActionResult Profile()
        {
            return View();
        }
    }
}
