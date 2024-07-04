using Bitirme.Areas.Identity.Data;
using Bitirme.Migrations;
using Bitirme.Models;
using Iyzipay.Model.V2.Subscription;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
using NuGet.Protocol.VisualStudio;
using System.ComponentModel.Design;
using static Bitirme.Areas.Sessions.Pages.MyOrderModel;

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
        public IActionResult Profile(string? receiverId)
        {
            ApplicationUser user = new ApplicationUser();
            var comments = new List<Bitirme.Models.Comment>();
            var currentUserId = _userManager.GetUserId(User);

            if (receiverId != null && receiverId != currentUserId)
            {
                user = _context.AspNetUsers.FirstOrDefault(x => x.Id == receiverId);
                comments = _context.Comments.Where(c => c.ReceiverUserID == receiverId).ToList();
                ViewBag.IsCurrUser = 0;
            }
            else if (receiverId == null)
            {
                user = _context.AspNetUsers.FirstOrDefault(x => x.Id == currentUserId);
                comments = _context.Comments.Where(c => c.ReceiverUserID == currentUserId).ToList();
                ViewBag.IsCurrUser = 1;
            }

            ViewBag.UserName = user.Ad + " " + user.Soyad;
            ViewBag.City = user.City;
            ViewBag.Country = user.Country;
            ViewBag.ProfileDescription = user.ProfileDescription;
            ViewBag.TransportNumber = user.TransportNumber;
            ViewBag.AdvertNumber = user.AdvertNumber;
            ViewBag.DeliverNumber = user.DeliverNumber;
            ViewBag.ProfilePicture = user.ProfilePicture;

            return View(comments);
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
        [HttpPost]
        public IActionResult AddComment(int ProductId, string commentText, int rating)
        {
            if (ModelState.IsValid)
            {
                var product = _context.tbl_products.FirstOrDefault(x => x.ProductId == ProductId);
                product.isCommentMade = 1;
                var transportUser = _context.AspNetUsers.FirstOrDefault(x => x.Id == product.TransporterUserId);
                var user = _context.AspNetUsers.FirstOrDefault(x => x.Id == product.UserId);
                var comment = new Comment
                {
                    CommentText = commentText,
                    SenderUserID = _userManager.GetUserId(User),
                    SenderUserName = _userManager.GetUserName(User),
                    ReceiverUserName = transportUser.UserName,
                    When = DateTime.Now,
                    ReceiverUserID = transportUser.Id,
                    Rating = rating,
                    ProductId = ProductId,
                    senderNameSurname = user.Ad + " " + user.Soyad,
                    senderProfilePicture = user.ProfilePicture
                };

                transportUser.RatingCount = transportUser.RatingCount + 1;
                transportUser.TotalRating = transportUser.TotalRating + rating;

                _context.Comments.Add(comment);
                _context.SaveChanges();

                return RedirectToAction("Profile");
            }

            return Ok();
        }

        [HttpPost]
        public IActionResult UpdateComment(int CommentId, string commentText, int rating)
        {
            var comment = _context.Comments.FirstOrDefault(x => x.CommentId == CommentId);
            var transportUser = _context.AspNetUsers.FirstOrDefault(x => x.Id == comment.ReceiverUserID);

            transportUser.TotalRating = transportUser.TotalRating - Convert.ToInt32(comment.Rating) + rating;

            comment.CommentText = commentText;
            comment.Rating = rating;

            _context.SaveChanges();

            return RedirectToAction("Profile");
        }

        [HttpPost]
        public IActionResult TransferPaid([FromBody] TransferPaidModel model)
        {
            var userId = _userManager.GetUserId(User);

            var user = _context.AspNetUsers.FirstOrDefault(x => x.Id == userId);
            if (user != null)
            {
                if (model.iban == null)
                {
                    ModelState.AddModelError("model.iban", "Geçersiz IBAN numarası.");
                    return View("Profile");
                }

                user.PaidPrice = user.PaidPrice - model.transferPaid;
                user.UserIBAN = model.iban;

                var transaction = new MoneyTransaction
                {
                    UserId = user.Id,
                    IBAN = model.iban,
                    Amount = model.transferPaid,
                    Date = DateTime.Now,
                    isPaid = 0,
                };

                _context.MoneyTransaction.Add(transaction);
                _context.SaveChanges();
            }
            return Json(model);
        }

        public class TransferPaidModel
        {
            public int transferPaid { get; set; }
            public string iban { get; set; }

        }

    }
}
