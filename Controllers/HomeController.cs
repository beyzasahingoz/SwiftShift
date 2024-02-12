using Bitirme.Areas.Identity.Data;
using Bitirme.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using NuGet.Protocol.Plugins;

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
        public IActionResult Message(string receiverId,string receiverUsername)
        {
            var currentUser = _userManager.GetUserName(User);  
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.CurrentUserName = currentUser;
                ViewBag.ReceiverUserName = receiverUsername;
                ViewBag.ReceiverUserID = receiverId;
            }
            var messages = _context.Messages.FromSqlRaw("select * from messages where (SenderUserName = {0} and ReceiverUserName = {1}) " +
                "or (SenderUserName = {1} and ReceiverUserName = {0})", currentUser, receiverUsername).ToList();
            return View(messages); 
        }

        public async Task<IActionResult> Create(Models.Message message)
        {
            if (ModelState.IsValid)
            {
                message.SenderUserName = User.Identity.Name;
                var sender = await _userManager.GetUserAsync(User);
                message.SenderUserID = sender.Id;

                await _context.Messages.AddAsync(message);
                await _context.SaveChangesAsync();

                TempData["receiverId"] = message.ReceiverUserID;
                TempData["receiverUsername"] = message.ReceiverUserName;

                return RedirectToAction("Message", new
                {
                    receiverId = message.ReceiverUserID,
                    receiverUsername = message.ReceiverUserName
                });
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