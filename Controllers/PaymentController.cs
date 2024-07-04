using Bitirme.Models;
using Microsoft.AspNetCore.Mvc;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Bitirme.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace Bitirme.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ILogger<PaymentController> _logger;

        private readonly DbContextSwiftShift _context;

        private readonly UserManager<ApplicationUser> _userManager;

        public PaymentController(ILogger<PaymentController> logger, DbContextSwiftShift context, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;

        }

        //string email, string isim, string soyisim, string tc, string sehir, string ilce, string adres
        public ActionResult Payment(int ProductID, string SenderUserID, string ReceiverUserID, int TravelID, int ChatID)
        {
            string token = "";
            Options options = new Options();
            options.ApiKey = "sandbox-a21qIienSHPjF86nOzuLu4enl8eTfAx1";
            options.SecretKey = "sandbox-OMeQUm7gvZ9fnAZtPwp7IEd6frHo4tfT";
            options.BaseUrl = "https://sandbox-api.iyzipay.com";

            Product order = _context.tbl_products.FirstOrDefault(x => x.ProductId == ProductID);
            ApplicationUser user = _context.AspNetUsers.FirstOrDefault(x => x.Id == SenderUserID);

            if (order.Amount == 0)
            {
                Travel travel = _context.Travel.FirstOrDefault(x => x.TravelId == TravelID);
                order.Amount = travel.pricePerKg * Convert.ToDouble(order.ProductKg);
                _context.SaveChanges();
            }

            CreateCheckoutFormInitializeRequest request = new CreateCheckoutFormInitializeRequest();
            request.Locale = Locale.TR.ToString();
            request.ConversationId = order.ProductId.ToString();
            request.Price = order.Amount.ToString();
            request.PaidPrice = order.Amount.ToString();
            request.Currency = Currency.TRY.ToString();
            //request.BasketId = order.BasketId.ToString();
            request.PaymentGroup = PaymentGroup.PRODUCT.ToString();
            request.CallbackUrl = Url.Action("CallBack", "Payment", new { id = order.ProductId, token = token, senderUserId = SenderUserID, receiverUserId = ReceiverUserID, productId = ProductID, chatId = ChatID }, Request.Scheme);
            request.Buyer = new Buyer();
            request.Buyer.Id = user.Id.ToString();
            request.Buyer.Name = user.Ad;
            request.Buyer.Surname = user.Soyad;
            request.Buyer.Email = user.Email;
            request.Buyer.IdentityNumber = user.TCKN;
            request.Buyer.RegistrationAddress = user.Address;
            request.Buyer.City = getCity(user.CityId);
            request.Buyer.Country = "Türkiye";

            request.BillingAddress = new Address
            {
                ContactName = user.Ad + " " + user.Soyad,
                City = getCity(user.CityId),
                Country = "Türkiye",
                Description = user.Address
            };

            // Ben her siparişte tek bir olacak şekilde ayarladığım için sepette tek ürün var o da siparişin kendisi.
            request.BasketItems = new List<BasketItem>
            {
                new BasketItem
                {
                    Id = order.ProductId.ToString(),
                    Name = order.ProductName,
                    Category1 = order.ProductName,
                    ItemType = BasketItemType.VIRTUAL.ToString(),
                    Price = order.Amount.ToString()+".00", // Sonuna .00 eklemek şart.
                },
            };

            CheckoutFormInitialize checkoutFormInitialize = CheckoutFormInitialize.Create(request, options);
            ViewBag.Iyzico = checkoutFormInitialize.CheckoutFormContent;
            token = checkoutFormInitialize.Token;

            //order.token = checkoutFormInitialize.Token; // Form'a ait token'ı kayıt ederek sonradan ödeme kontrolü yaparken kullancağız.
            //_educationOrderManager.Update(productOrder);
            return View();
        }

        public async Task<IActionResult> CallBack(string id, string token, string senderUserId, string receiverUserId, int productId, int chatId)
        {

            Options options = new Options();
            options.ApiKey = "sandbox-a21qIienSHPjF86nOzuLu4enl8eTfAx1";
            options.SecretKey = "sandbox-OMeQUm7gvZ9fnAZtPwp7IEd6frHo4tfT";
            options.BaseUrl = "https://sandbox-api.iyzipay.com";

            var order = _context.tbl_products.FirstOrDefault(x => x.ProductId == Convert.ToInt32(id));

            RetrieveCheckoutFormRequest request = new RetrieveCheckoutFormRequest();
            request.Locale = Locale.TR.ToString();
            request.ConversationId = id;
            request.Token = token;

            CheckoutForm checkoutForm = CheckoutForm.Retrieve(request, options);

            if (checkoutForm.PaymentStatus == "SUCCESS")
            {
                TempData["PaymentMessage"] = " ";

                Product product = _context.tbl_products.FirstOrDefault(x => x.ProductId == productId);
                ApplicationUser receiverUser = _context.AspNetUsers.FirstOrDefault(x => x.Id == receiverUserId);
                ApplicationUser senderUser = _context.AspNetUsers.FirstOrDefault(x => x.Id == senderUserId);
                MessageInfo info = _context.MessageInfo.FirstOrDefault(x => x.chatId == chatId);
                Travel travel = _context.Travel.FirstOrDefault(x => x.TravelId == info.travelId);

                travel.isActiveCarrier = 1;
                product.isOrder = 1;
                product.TransporterUserId = receiverUser.Id;

                int transportNumber = int.Parse(senderUser.TransportNumber);
                transportNumber++;
                senderUser.TransportNumber = transportNumber.ToString();

                info.isPaid = 1;
                _context.SaveChanges();

                return LocalRedirect("/Home/Index");
            }
            else
            {
                return BadRequest("Ödeme başarısız oldu!");
            }
        }

        private string getCity(int cityId)
        {
            return _context.Cities
                 .Where(c => c.Id == cityId)
                 .OrderBy(n => n.CityName)
                 .Select(c => c.CityName).ToList()[0];
        }
    }
}
