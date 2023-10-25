using Bitirme.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bitirme.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return _context.Users != null ?
                        View(await _context.Users.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Users'  is null.");
        }

        [HttpPost]
        public async Task<IActionResult> Login(Models.Login model)
        {
            if (ModelState.IsValid)
            {
                var User = from m in _context.Users select m;
                User = User.Where(s => s.Name.Contains(model.Username));
                if (User.Count() != 0)
                {
                    if (User.First().Password == model.Password)
                    {
                        return RedirectToAction("Success");
                    }
                }
            }
            return RedirectToAction("Fail");
        }

        public IActionResult Success()
        {
            return View();
        }

        public IActionResult Fail()
        {
            return View();
        }
    }
}
