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
            var explores = _context.tbl_products.ToList();
            var exploreModels = new List<Explore>();
            foreach (var product in explores)
            {
                var city = getCity(product.CityId);
                var country = getCountry(product.CountryId);
                exploreModels.Add(new Explore
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Country = country,
                    City = city,
                    Address = product.Address,
                    ProductKg = product.ProductKg,
                    Note = product.ProductNote
                });
            }
            return View(exploreModels);
        }
        private string getCountry(int countryId)
        {
            return _context.Countries
                 .Where(c => c.Id == countryId)
                 .OrderBy(n => n.CountryName)
                 .Select(c => c.CountryName).ToList()[0];
        }
        private string getCity(int cityId)
        {
            return _context.Cities
                 .Where(c => c.Id == cityId)
                 .OrderBy(n => n.CityName)
                 .Select(c => c.CityName).ToList()[0];
        }
        public async Task<IActionResult> Message(string receiverId,string receiverUsername)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.CurrentUserName = currentUser.UserName;
            }
            var list = new List<string>() { receiverUsername, currentUser.UserName };
            var messages = await _context.Messages.Where(x => list.Contains(x.SenderUserName)).ToListAsync();
            //var sql = string.Format("SELECT * FROM [Messages] WHERE SenderUserName IN('{0}', '{1}')", currentUser.UserName, receiverUsername);
            //var messages = _context.Messages.FromSqlRaw(sql).ToListAsync();
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