using Bitirme.Areas.Identity.Data;
using Bitirme.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Bitirme.Controllers
{
    [Authorize]

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly DbContextSwiftShift _context;

        private readonly UserManager<ApplicationUser> _userManager;
        public HomeController(ILogger<HomeController> logger, DbContextSwiftShift context, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Message(string receiverId,string receiverUsername)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.CurrentUserName = currentUser.UserName;
            }
            var messages = await _context.Messages.ToListAsync();
            return View(messages); 
        }

        public async Task<IActionResult> Create(Message message)
        {
            if (ModelState.IsValid)
            {
                message.SenderUserName = User.Identity.Name;
                var sender = await _userManager.GetUserAsync(User);
                message.SenderUserID = sender.Id;
                await _context.Messages.AddAsync(message);
                await _context.SaveChangesAsync();
                var messages = await _context.Messages.ToListAsync();
                return View("Message",messages);
            }
            return Error();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        
        public List<ApplicationUser> GetAllLocation()
        {
            string userId = _userManager.GetUserId(User);
            
            return _context.AspNetUsers.Where(x => x.Id != userId).ToList();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}