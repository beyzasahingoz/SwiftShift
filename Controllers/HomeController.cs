using Bitirme.Areas.Identity.Data;
using Bitirme.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using Bitirme.Migrations;
using NuGet.Protocol.Plugins;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.AspNetCore.Http;

namespace Bitirme.Controllers
{
    [Authorize]

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly DbContextSwiftShift _context;

        private readonly UserManager<ApplicationUser> _userManager;

        public CascadingModel CascadingDepartureModel { get; set; }
        public List<SelectListItem> DepartureCities { get; set; }

        public CascadingModel CascadingArrivalModel { get; set; }
        public List<SelectListItem> ArrivalCities { get; set; }

        public const string SessionChatId = "ChatId";
        public const string SessionType = "MessageType";

        public HomeController(ILogger<HomeController> logger, DbContextSwiftShift context, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;

            CascadingModel CascadingDepartureModel = new CascadingModel();
            CascadingDepartureModel.Cities = GetDepartureCities();

            CascadingModel CascadingArrivalModel = new CascadingModel();
            CascadingArrivalModel.Cities = GetArrivalCities();
        }

        [HttpPost]
        public IActionResult SendOfferMessage(string offerAmount, string senderUserName, string receiverUserName, string senderUserId, string receiverUserId, int chatId, int productId, int travelId)
        {
            var message = new Models.Message
            {
                SenderUserName = senderUserName,
                ReceiverUserName = receiverUserName,
                SenderUserID = senderUserId,
                ReceiverUserID = receiverUserId,
                Text = $"{offerAmount} TL teklif gönderildi.",
                IsOfferMessage = true,
                Amount = Convert.ToDouble(offerAmount),
                chatId = chatId,
                ProductId = productId,
                TravelId = travelId
            };

            _context.Messages.Add(message);
            _context.SaveChanges();

            return Ok();
        }
        [HttpPost]
        public IActionResult AcceptOfferMessage(string offerAmount, string senderUserName, string receiverUserName, string senderUserId, string receiverUserId, int productId, int travelId, int chatId)
        {
            var message = new Models.Message
            {
                SenderUserName = senderUserName,
                ReceiverUserName = receiverUserName,
                SenderUserID = senderUserId,
                ReceiverUserID = receiverUserId,
                Text = $"{offerAmount} TL teklifi kabul edildi!",
                IsOfferMessage = false,
                chatId = chatId,
                ProductId = productId,
                TravelId = travelId
            };
            
            var messages = _context.Messages.Where(x => x.chatId == chatId).ToList();
            var offerMessages = messages.Where(m => m.IsOfferMessage == true).ToList();
            var lastOfferMessage = offerMessages.LastOrDefault();
            lastOfferMessage.IsOfferMessage = false;

            Product p = _context.tbl_products.FirstOrDefault(x => x.ProductId == productId);
            p.Amount = Convert.ToDouble(offerAmount);

            _context.Messages.Add(message);
            _context.SaveChanges();

            HttpContext.Session.SetInt32(SessionChatId, chatId);
            HttpContext.Session.SetString(SessionType, "box");

            return RedirectToAction("Message");
        }
        public IActionResult RejectOfferMessage(string offerAmount, string senderUserName, string receiverUserName, string senderUserId, string receiverUserId, int chatId, int productId, int travelId)
        {
            var message = new Models.Message
            {

                SenderUserName = senderUserName,
                ReceiverUserName = receiverUserName,
                SenderUserID = senderUserId,
                ReceiverUserID = receiverUserId,
                Text = $"{offerAmount} TL teklifi reddedildi!",
                IsOfferMessage = false,
                chatId = chatId,
                ProductId = productId,
                TravelId = travelId
            };
            var messages = _context.Messages.Where(x => x.chatId == chatId).ToList();
            var offerMessages = messages.Where(m => m.IsOfferMessage == true).ToList();
            var lastOfferMessage = offerMessages.LastOrDefault();
            lastOfferMessage.IsOfferMessage = false;

            _context.Messages.Add(message);
            _context.SaveChanges();

            HttpContext.Session.SetInt32(SessionChatId, chatId);
            HttpContext.Session.SetString(SessionType, "box");

            return RedirectToAction("Message");
        }

        public IActionResult Index()
        {
            ViewData["PaymentMessage"] = TempData["PaymentMessage"];

            string userId = _userManager.GetUserId(User);

            var products = _context.tbl_products.Where(x => x.UserId != userId && x.isOrder == 0 && x.WhenDate >= DateTime.Now).ToList();

            for (int i = 0; i < products.Count; i++)
            {
                var FromCountry = getCountry(products[i].FromCountryId);
                products[i].FromCountry = new Country();
                products[i].FromCountry.CountryName = FromCountry;

                var FromCity = getCity(products[i].FromCityId);
                products[i].FromCity = new City();
                products[i].FromCity.CityName = FromCity;

                var FromDistrict = getDistrict(products[i].FromDistrictId);
                products[i].FromDistrict = new District();
                products[i].FromDistrict.DistrictName = FromDistrict;

                var ToCountry = getCountry(products[i].ToCountryId);
                products[i].ToCountry = new Country();
                products[i].ToCountry.CountryName = ToCountry;

                var ToCity = getCity(products[i].ToCityId);
                products[i].ToCity = new City();
                products[i].ToCity.CityName = ToCity;

                var ToDistrict = getDistrict(products[i].ToDistrictId);
                products[i].ToDistrict = new District();
                products[i].ToDistrict.DistrictName = ToDistrict;
            }

            ViewBag.DepartureCities = GetDepartureCities();
            ViewBag.ArrivalCities = GetArrivalCities();

            return View(products);
        }

        [AllowAnonymous]
        public IActionResult Welcome()
        {
            return View();
        }
        public IActionResult GetAllUserProduct()
        {
            string userId = _userManager.GetUserId(User);

            var products = _context.tbl_products.Where(x => x.UserId == userId && x.isOrder == 0).ToList();

            for (int i = 0; i < products.Count; i++)
            {
                var FromCountry = getCountry(products[i].FromCountryId);
                products[i].FromCountry = new Country();
                products[i].FromCountry.CountryName = FromCountry;

                var FromCity = getCity(products[i].FromCityId);
                products[i].FromCity = new City();
                products[i].FromCity.CityName = FromCity;

                var FromDistrict = getDistrict(products[i].FromDistrictId);
                products[i].FromDistrict = new District();
                products[i].FromDistrict.DistrictName = FromDistrict;

                var ToCountry = getCountry(products[i].ToCountryId);
                products[i].ToCountry = new Country();
                products[i].ToCountry.CountryName = ToCountry;

                var ToCity = getCity(products[i].ToCityId);
                products[i].ToCity = new City();
                products[i].ToCity.CityName = ToCity;

                var ToDistrict = getDistrict(products[i].ToDistrictId);
                products[i].ToDistrict = new District();
                products[i].ToDistrict.DistrictName = ToDistrict;
            }

            return Json(products);
        }

        public IActionResult GetAllUserTravel()
        {
            string userId = _userManager.GetUserId(User);

            var travels = _context.Travel.Where(x => x.UserId == userId).ToList();

            for (int i = 0; i < travels.Count; i++)
            {
                var DepartureCountry = getCountry(travels[i].DepartureCountryId);
                travels[i].DepartureCountry = new Country();
                travels[i].DepartureCountry.CountryName = DepartureCountry;

                var DepartureCity = getCity(travels[i].DepartureCityId);
                travels[i].DepartureCity = new City();
                travels[i].DepartureCity.CityName = DepartureCity;

                var DepartureDistrict = getDistrict(travels[i].DepartureDistrictId);
                travels[i].DepartureDistrict = new District();
                travels[i].DepartureDistrict.DistrictName = DepartureDistrict;

                var ArrivalCountry = getCountry(travels[i].ArrivalCountryId);
                travels[i].ArrivalCountry = new Country();
                travels[i].ArrivalCountry.CountryName = ArrivalCountry;

                var ArrivalCity = getCity(travels[i].ArrivalCityId);
                travels[i].ArrivalCity = new City();
                travels[i].ArrivalCity.CityName = ArrivalCity;

                var ArrivalDistrict = getDistrict(travels[i].ArrivalDistrictId);
                travels[i].ArrivalDistrict = new District();
                travels[i].ArrivalDistrict.DistrictName = ArrivalDistrict;
            }
            return Json(travels);
        }

        public List<ApplicationUser> GetPreviousContacts()
        {
            string userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return new List<ApplicationUser>();
            }

            var previousMessages = _context.MessageInfo
        .Where(m => (m.transportUserId == userId || m.customerUserId == userId) && m.isCompleted == 0).ToList();

            if (previousMessages == null || !previousMessages.Any())
            {
                return new List<ApplicationUser>();
            }

            var previousContactsIds = previousMessages
        .Select(m => m.customerUserId != userId ? m.customerUserId : m.transportUserId)
        .Distinct()
        .ToList();

            var previousContacts = _context.AspNetUsers
                .Where(u => previousContactsIds.Contains(u.Id) && u.Id != userId)
                .ToList();

            return previousContacts;
        }

        public async Task<IActionResult> Create(Models.Message message)
        {
            if (!ModelState.IsValid)
            {
                message.SenderUserName = User.Identity.Name;
                var sender = await _userManager.GetUserAsync(User);
                message.SenderUserID = sender.Id;

                await _context.Messages.AddAsync(message);
                await _context.SaveChangesAsync();

                TempData["receiverId"] = message.ReceiverUserID;
                TempData["receiverUsername"] = message.ReceiverUserName;
                TempData["isTransporter"] = message.isTransporter;
                TempData["productId"] = message.ProductId;
                TempData["travelId"] = message.TravelId;
                TempData["chatId"] = message.chatId;
                TempData["type"] = "box";

                HttpContext.Session.SetInt32(SessionChatId, message.chatId);
                HttpContext.Session.SetString(SessionType, "box");

                return RedirectToAction("Message");
            }
            return Error();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult AddMessageInfo(string? transportUserId, string? customerUserId, int productId, int travelId)
        {
            MessageInfo messageInfo = new MessageInfo();
            string currUserId = _userManager.GetUserId(User);

            if (transportUserId != null)
            {
                var messages = _context.MessageInfo
                    .FirstOrDefault(m => m.transportUserId == transportUserId && m.productId == productId && m.travelId == travelId);

                if (messages == null)
                {
                    messageInfo.productId = productId;
                    messageInfo.travelId = travelId;
                    messageInfo.transportUserId = transportUserId;
                    messageInfo.customerUserId = currUserId;
                    messageInfo.isCompleted = 0;
                    messageInfo.isPaid = 0;

                    _context.MessageInfo.Add(messageInfo);
                    _context.SaveChanges();

                    HttpContext.Session.SetInt32(SessionChatId, messageInfo.chatId);
                    HttpContext.Session.SetString(SessionType, "transporter");
                    return RedirectToAction("Message");
                }
                //else
                //{
                //    var info = _context.MessageInfo.FirstOrDefault(x => x.transportUserId == transportUserId && x.customerUserId == currUserId);
                //    if (info.productId == productId || info.travelId == travelId)
                //    {
                //        return RedirectToAction("Message", new { chatId = info.chatId });
                //    }
                //}
            }
            else if (customerUserId != null)
            {
                var messages = _context.MessageInfo
                   .FirstOrDefault(m => m.customerUserId == customerUserId && m.productId == productId && m.travelId == travelId);

                if (messages == null)
                {
                    messageInfo.productId = productId;
                    messageInfo.travelId = travelId;
                    messageInfo.transportUserId = currUserId;
                    messageInfo.customerUserId = customerUserId;
                    messageInfo.isCompleted = 0;
                    messageInfo.isPaid = 0;

                    _context.MessageInfo.Add(messageInfo);
                    _context.SaveChanges();

                    HttpContext.Session.SetInt32(SessionChatId, messageInfo.chatId);
                    HttpContext.Session.SetString(SessionType, "first");
                    return RedirectToAction("Message");
                }
                //else
                //{
                //    var info = _context.MessageInfo.FirstOrDefault(x => x.customerUserId == customerUserId && x.transportUserId == currUserId);
                //    if (info.productId == productId || info.travelId == travelId)
                //    {
                //        return RedirectToAction("Message", new { chatId = info.chatId, type = "first" });
                //    }
                //}
            }
            return RedirectToAction("Index");
        }

        public IActionResult Message()
        {
            string currUserId = _userManager.GetUserId(User);

            int chatId = (int)HttpContext.Session.GetInt32(SessionChatId);
            string type = HttpContext.Session.GetString(SessionType);

            if (chatId == 0 && type == "")
            {
                ViewBag.MessageBox = 1;
                ViewBag.isDeleted = 0;
                var previouscontacts = GetPreviousContacts();
                ViewBag.PreviousContacts = previouscontacts;
                return View();
                //return RedirectToAction("Index", "Home");
            }

            if (chatId == 0 && type == "deleted")
            {
                ViewBag.MessageBox = 1;
                ViewBag.isDeleted = 1;
                var previouscontacts = GetPreviousContacts();
                ViewBag.PreviousContacts = previouscontacts;
                return View();
            }

            var info = _context.MessageInfo.FirstOrDefault(x => x.chatId == chatId);
            var messages = _context.Messages.Where(x => x.chatId == chatId).ToList();

            if (type == "box")
            {
                var senderUserName = _context.AspNetUsers.FirstOrDefault(x => x.Id == currUserId);

                if (currUserId == info.customerUserId)
                {
                    var receiverUserName = _context.AspNetUsers.FirstOrDefault(x => x.Id == info.transportUserId);
                    ViewBag.SenderUserID = currUserId;
                    ViewBag.SenderUserAdSoyad = senderUserName.Ad + " " + senderUserName.Soyad;
                    ViewBag.SenderUserName = senderUserName.UserName;
                    ViewBag.ReceiverUserID = info.transportUserId;
                    ViewBag.ReceiverUserAdSoyad = receiverUserName.Ad + " " + receiverUserName.Soyad;
                    ViewBag.ReceiverUserName = receiverUserName.UserName;
                    ViewBag.ReceiverProfilePicture = receiverUserName.ProfilePicture;
                }
                else if (currUserId == info.transportUserId)
                {
                    var receiverUserName = _context.AspNetUsers.FirstOrDefault(x => x.Id == info.customerUserId);
                    ViewBag.SenderUserID = currUserId;
                    ViewBag.SenderUserAdSoyad = senderUserName.Ad + " " + senderUserName.Soyad;
                    ViewBag.SenderUserName = senderUserName.UserName;
                    ViewBag.ReceiverUserID = info.customerUserId;
                    ViewBag.ReceiverUserAdSoyad = receiverUserName.Ad + " " + receiverUserName.Soyad;
                    ViewBag.ReceiverUserName = receiverUserName.UserName;
                    ViewBag.ReceiverProfilePicture = receiverUserName.ProfilePicture;
                }
            }
            else if (type == "transporter")
            {
                var senderUserName = _context.AspNetUsers.FirstOrDefault(x => x.Id == info.customerUserId);
                var receiverUserName = _context.AspNetUsers.FirstOrDefault(x => x.Id == info.transportUserId);

                ViewBag.SenderUserID = info.customerUserId;
                ViewBag.SenderUserAdSoyad = senderUserName.Ad + " " + senderUserName.Soyad;
                ViewBag.SenderUserName = senderUserName.UserName;
                ViewBag.ReceiverUserID = info.transportUserId;
                ViewBag.ReceiverUserAdSoyad = receiverUserName.Ad + " " + receiverUserName.Soyad;
                ViewBag.ReceiverUserName = receiverUserName.UserName;
                ViewBag.ReceiverProfilePicture = receiverUserName.ProfilePicture;
            }
            else
            {
                var senderUserName = _context.AspNetUsers.FirstOrDefault(x => x.Id == info.transportUserId);
                var receiverUserName = _context.AspNetUsers.FirstOrDefault(x => x.Id == info.customerUserId);

                ViewBag.SenderUserID = info.transportUserId;
                ViewBag.SenderUserAdSoyad = senderUserName.Ad + " " + senderUserName.Soyad;
                ViewBag.SenderUserName = senderUserName.UserName;
                ViewBag.ReceiverUserID = info.customerUserId;
                ViewBag.ReceiverUserAdSoyad = receiverUserName.Ad + " " + receiverUserName.Soyad;
                ViewBag.ReceiverUserName = receiverUserName.UserName;
                ViewBag.ReceiverProfilePicture = receiverUserName.ProfilePicture;
            }

            var product = _context.tbl_products.FirstOrDefault(x => x.ProductId == info.productId);

            ViewBag.chatId = chatId;
            ViewBag.IsTransporter = info.customerUserId == currUserId ? 0 : 1;
            ViewBag.ProductId = info.productId;
            ViewBag.TravelId = info.travelId;
            ViewBag.IsCompleted = info.isCompleted;
            ViewBag.IsPaid = info.isPaid;
            ViewBag.MessageBox = 0;
            ViewBag.isDeleted = 0;
            ViewBag.isOrder = product.isOrder;
            ViewBag.isDelivered = product.isDelivered;


            var previousContacts = GetPreviousContacts();
            ViewBag.PreviousContacts = previousContacts;

            var offerMessages = messages.Where(m => m.IsOfferMessage == true).ToList();
            var lastOfferMessage = offerMessages.LastOrDefault();
            if (lastOfferMessage != null)
            {
                ViewBag.LastOffer = lastOfferMessage.IsOfferMessage ? 0 : 1;
            }
            else
            {
                ViewBag.LastOffer = 1;
            }
            double lastOfferAmount = lastOfferMessage?.Amount ?? 0;
            ViewBag.OfferAmount = lastOfferAmount;

            return View(messages);
        }

        public IActionResult MessageBox()
        {
            //string currUserId = _userManager.GetUserId(User);

            //var info = _context.MessageInfo
            //    .Where(x => (x.customerUserId == currUserId || x.transportUserId == currUserId) && x.isCompleted == 0)
            //    .FirstOrDefault();

            //if (info != null)
            //{
            //    HttpContext.Session.SetInt32(SessionChatId, info.chatId);
            //    HttpContext.Session.SetString(SessionType, "box");

            //    return RedirectToAction("Message");
            //}
            //else
            //{
            //    TempData["ErrorMessage"] = "Mesaj kutunuzda hiç mesaj yok.";
            //    return RedirectToAction("Index", "Home");
            //}

            HttpContext.Session.SetInt32(SessionChatId, 0);
            HttpContext.Session.SetString(SessionType, "");
            return RedirectToAction("Message");
        }

        public IActionResult ContactMessage(string receiverId)
        {
            ViewBag.ReceiverId = receiverId;

            string currUserId = _userManager.GetUserId(User);

            var info = _context.MessageInfo
                .Where(x => ((x.customerUserId == currUserId && x.transportUserId == receiverId)
                || (x.transportUserId == currUserId && x.customerUserId == receiverId)) && x.isCompleted == 0)
                .FirstOrDefault();

            if (info != null)
            {
                var product = _context.tbl_products.FirstOrDefault(x => x.ProductId == info.productId);
                var travel = _context.Travel.FirstOrDefault(x => x.TravelId == info.travelId);

                if (product == null || travel == null)
                {
                    info.isCompleted = 1;
                    HttpContext.Session.SetInt32(SessionChatId, 0);
                    HttpContext.Session.SetString(SessionType, "deleted");
                    _context.SaveChanges();
                }
                else
                {
                    HttpContext.Session.SetInt32(SessionChatId, info.chatId);
                    HttpContext.Session.SetString(SessionType, "box");
                }

                return RedirectToAction("Message");
            }
            else
            {
                TempData["ErrorMessage"] = "Mesaj kutunuzda hiç mesaj yok.";
                return RedirectToAction("Index", "Home");
            }
        }


        public void GetFinishChat(int chatId)
        {
            var infos = _context.MessageInfo.FirstOrDefault(x => x.chatId == chatId);

            infos.isCompleted = 1;
            _context.SaveChanges();
        }

        public int GetCheckProductChat(string receiverId)
        {
            string currUserId = _userManager.GetUserId(User);

            var infos = _context.MessageInfo
                        .FromSqlRaw("SELECT * FROM MessageInfo WHERE ((transportUserId = {0} AND customerUserId = {1})" +
                        " OR (transportUserId = {1} AND customerUserId = {0})) AND isCompleted = 0", receiverId, currUserId)
                        .ToList();

            return infos.Count == 0 ? 0 : 1;
        }

        public int GetCheckTravelChat(string receiverId)
        {
            string currUserId = _userManager.GetUserId(User);

            var infos = _context.MessageInfo
                        .FromSqlRaw("SELECT * FROM MessageInfo WHERE ((transportUserId = {0} AND customerUserId = {1})" +
                        " OR (transportUserId = {1} AND customerUserId = {0})) AND isCompleted = 0", receiverId, currUserId)
                        .ToList();

            return infos.Count == 0 ? 0 : 1;
        }

        public string GetProductTravelInfo(int productId, int travelId)
        {
            Product product = _context.tbl_products.FirstOrDefault(x => x.ProductId == productId);
            Travel travel = _context.Travel.FirstOrDefault(x => x.TravelId == travelId);

            var productDeparture = getDistrict(product.FromDistrictId) + "/" + getCity(product.FromCityId) + "/" + getCountry(product.FromCountryId);
            var productArrival = getDistrict(product.ToDistrictId) + "/" + getCity(product.ToCityId) + "/" + getCountry(product.ToCountryId);

            DateTime whenDate = product.WhenDate;

            var travelDeparture = getDistrict(travel.DepartureDistrictId) + "/" + getCity(travel.DepartureCityId) + "/" + getCountry(travel.DepartureCountryId);
            var travelArrival = getDistrict(travel.ArrivalDistrictId) + "/" + getCity(travel.ArrivalCityId) + "/" + getCountry(travel.ArrivalCountryId);

            DateTime DepartureDate = travel.DepartureDate;
            DateTime ArrivalDate = travel.ArrivalDate;

            return String.Format(
                     "ÜRÜN BİLGİLERİ:\n" +
                     "Adı                 : {0}\n" +
                     "Kilogramı           : {1} kg\n" +
                     "Bulunduğu Yer       : {2}\n" +
                     "Gideceği Yer        : {3}\n" +
                     "Adres               : {4}\n" +
                     "Gideceği Tarih      : {5}\n" +
                     "----------------------------" +
                     "\nSEYAHAT BİLGİLERİ:\n" +
                     "Kalkış Yeri         : {6}\n" +
                     "Gideceği Yer        : {7}\n" +
                     "Kalkış Tarihi       : {8}\n" +
                     "Varış Tarihi        : {9}\n" +
                     "Max. Kilogram       : {10} kg\n" +
                     "Kg Başına Ücret     : {11} TL",
                     product.ProductName,
                     product.ProductKg,
                     productDeparture,
                     productArrival,
                     product.Address,
                     whenDate.ToString("dd/MM/yyyy"),
                     travelDeparture,
                     travelArrival,
                     DepartureDate.ToString("dd/MM/yyyy"),
                     ArrivalDate.ToString("dd/MM/yyyy"),
                     travel.MaxWeightCarry,
                     travel.pricePerKg
                 );
        }

        public Array GetAllLocation(int departureCity, int departureDistrict, int arrivalCity, int arrivalDistrict, string minDepartureDate, string maxDepartureDate, int seeProduct, int carrySensitiveProduct)
        {
            string userId = _userManager.GetUserId(User);
            DateTime? MinDepartureDate = null;
            DateTime? MaxDepartureDate = null;

            if (!string.IsNullOrEmpty(minDepartureDate))
            {
                MinDepartureDate = DateTime.ParseExact(minDepartureDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }

            if (!string.IsNullOrEmpty(maxDepartureDate))
            {
                MaxDepartureDate = DateTime.ParseExact(maxDepartureDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }

            var sortedData = (
                from user in _context.AspNetUsers
                join travel in _context.Travel on user.Id equals travel.UserId
                where travel.UserId != userId && travel.DepartureDate >= DateTime.Now
                && (!string.IsNullOrEmpty(departureCity.ToString()) && departureCity != 0 ? travel.DepartureCityId == departureCity : true)
                && (!string.IsNullOrEmpty(departureDistrict.ToString()) && departureDistrict != 0 ? travel.DepartureDistrictId == departureDistrict : true)
                && (!string.IsNullOrEmpty(arrivalCity.ToString()) && arrivalCity != 0 ? travel.ArrivalCityId == arrivalCity : true)
                && (!string.IsNullOrEmpty(arrivalDistrict.ToString()) && arrivalDistrict != 0 ? travel.ArrivalDistrictId == arrivalDistrict : true)
                && (MinDepartureDate.HasValue ? travel.DepartureDate >= MinDepartureDate : true)
                && (MaxDepartureDate.HasValue ? travel.ArrivalDate <= MaxDepartureDate : true)
                && (seeProduct == 1 ? travel.seeProduct == 1 : true)
                && (carrySensitiveProduct == 1 ? travel.carrySensitiveProduct == 1 : true)
                orderby travel.DepartureDate ascending
                select new
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Ad = user.Ad,
                    Soyad = user.Soyad,
                    Profilepicture = user.ProfilePicture,
                    ProfileDescription = user.ProfileDescription,
                    Point = _context.Comments
                            .Where(comment => comment.ReceiverUserID == user.Id)
                            .Select(comment => (double?)comment.Rating)
                            .DefaultIfEmpty()
                            .Average() ?? 0,
                    Latitude = user.Latitude,
                    Longitude = user.Longitude,
                    TravelId = travel.TravelId,
                    ArrivalCity = travel.ArrivalCity.CityName,
                    ArrivalDistrict = travel.ArrivalDistrict.DistrictName,
                    DepartureCity = travel.DepartureCity.CityName,
                    DepartureDistrict = travel.DepartureDistrict.DistrictName,
                    ArrivalDate = travel.ArrivalDate.ToString("dd/MM/yyyy"),
                    DepartureDate = travel.DepartureDate.ToString("dd/MM/yyyy"),
                    MaxWeightCarry = travel.MaxWeightCarry,
                    pricePerKg = travel.pricePerKg,
                    seeProduct = travel.seeProduct,
                    carrySensitiveProduct = travel.carrySensitiveProduct
                }).ToList();


            var distinctData = sortedData
                .GroupBy(x => x.Id)
                .Select(group => group.First())
                .ToArray();

            return distinctData;
        }

        private List<SelectListItem> GetDepartureCities()
        {
            DepartureCities = (from city in _context.Cities
                               select new SelectListItem
                               {
                                   Value = city.Id.ToString(),
                                   Text = city.CityName
                               }).ToList();

            return DepartureCities;
        }

        private List<SelectListItem> GetArrivalCities()
        {
            ArrivalCities = (from city in _context.Cities
                             select new SelectListItem
                             {
                                 Value = city.Id.ToString(),
                                 Text = city.CityName
                             }).ToList();

            return ArrivalCities;
        }

        public IActionResult AjaxMethod(string type, int value)
        {
            if (type == null)
            {
                return Ok();
            }
            if (type.Contains("Departure"))
            {
                CascadingDepartureModel = new CascadingModel();
                switch (type)
                {
                    case "ddlDepartureCities":
                        CascadingDepartureModel.District = (from district in _context.District
                                                            where district.CityId == value
                                                            select new SelectListItem
                                                            {
                                                                Value = district.Id.ToString(),
                                                                Text = district.DistrictName
                                                            }).ToList();
                        ViewBag.DepartureDistrict = CascadingDepartureModel.District;
                        break;
                }
                return new JsonResult(CascadingDepartureModel);
            }
            else
            {
                CascadingArrivalModel = new CascadingModel();
                switch (type)
                {
                    case "ddlArrivalCities":
                        CascadingArrivalModel.District = (from district in _context.District
                                                          where district.CityId == value
                                                          select new SelectListItem
                                                          {
                                                              Value = district.Id.ToString(),
                                                              Text = district.DistrictName
                                                          }).ToList();
                        ViewBag.ArrivalDistrict = CascadingArrivalModel.District;
                        break;
                }
                return new JsonResult(CascadingArrivalModel);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
        private string getDistrict(int districtId)
        {
            return _context.District
                 .Where(c => c.Id == districtId)
                 .OrderBy(n => n.DistrictName)
                 .Select(c => c.DistrictName).ToList()[0];
        }
    }
}