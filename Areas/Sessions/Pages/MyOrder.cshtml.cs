using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Bitirme.Areas.Identity.Data;
using Bitirme.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.SqlServer.Server;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Bitirme.Areas.Sessions.Pages
{
    [Authorize]
    public class MyOrderModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly DbContextSwiftShift _context;

        public List<SelectListItem> Cities { get; set; }
        public List<SelectListItem> District { get; set; }

        public List<Product> UserOrders { get; set; }
        public List<Product> ProductTransporter { get; set; }

        public MyOrderModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            DbContextSwiftShift context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userId = await _userManager.GetUserIdAsync(user);
            UserOrders = GetUserOrders(userId);
            ProductTransporter = GetProductTransporter(userId);
            var isOrder = await _context.tbl_products
                        .Where(p => p.UserId == userId && p.isOrder == 1)
                        .AnyAsync();

            ViewData["UserId"] = userId;
            ViewData["IsOrder"] = isOrder;
           
            return Page();
        }

        public JsonResult OnGetAjaxMethod(int id)
        {
            var selectedOrder = _context.tbl_products.FirstOrDefault(x => x.ProductId == id);

            var FromCountry = getCountry(selectedOrder.FromCountryId);
            selectedOrder.FromCountry = new Country();
            selectedOrder.FromCountry.CountryName = FromCountry;

            var FromCity = getCity(selectedOrder.FromCityId);
            selectedOrder.FromCity = new City();
            selectedOrder.FromCity.CityName = FromCity;

            var FromDistrict = getDistrict(selectedOrder.FromDistrictId);
            selectedOrder.FromDistrict = new District();
            selectedOrder.FromDistrict.DistrictName = FromDistrict;

            var ToCountry = getCountry(selectedOrder.ToCountryId);
            selectedOrder.ToCountry = new Country();
            selectedOrder.ToCountry.CountryName = ToCountry;

            var ToCity = getCity(selectedOrder.ToCityId);
            selectedOrder.ToCity = new City();
            selectedOrder.ToCity.CityName = ToCity;

            var ToDistrict = getDistrict(selectedOrder.ToDistrictId);
            selectedOrder.ToDistrict = new District();
            selectedOrder.ToDistrict.DistrictName = ToDistrict;

            return new JsonResult(selectedOrder);
        }

        public List<Product> GetProductTransporter(string userId)
        {
            var order = _context.tbl_products.Where(x => x.TransporterUserId == userId && x.isOrder == 1).ToList();

            for (int i = 0; i < order.Count; i++)
            {
                var FromCountry = getCountry(order[i].FromCountryId);
                order[i].FromCountry = new Country();
                order[i].FromCountry.CountryName = FromCountry;

                var FromCity = getCity(order[i].FromCityId);
                order[i].FromCity = new City();
                order[i].FromCity.CityName = FromCity;

                var FromDistrict = getDistrict(order[i].FromDistrictId);
                order[i].FromDistrict = new District();
                order[i].FromDistrict.DistrictName = FromDistrict;

                var ToCountry = getCountry(order[i].ToCountryId);
                order[i].ToCountry = new Country();
                order[i].ToCountry.CountryName = ToCountry;

                var ToCity = getCity(order[i].ToCityId);
                order[i].ToCity = new City();
                order[i].ToCity.CityName = ToCity;

                var ToDistrict = getDistrict(order[i].ToDistrictId);
                order[i].ToDistrict = new District();
                order[i].ToDistrict.DistrictName = ToDistrict;
            }
            return order;
        }

        public List<Product> GetUserOrders(string userId)
        {
            var order = _context.tbl_products.Where(x => x.UserId == userId && x.isOrder == 1).ToList();

            for (int i = 0; i < order.Count; i++)
            {
                var FromCountry = getCountry(order[i].FromCountryId);
                order[i].FromCountry = new Country();
                order[i].FromCountry.CountryName = FromCountry;

                var FromCity = getCity(order[i].FromCityId);
                order[i].FromCity = new City();
                order[i].FromCity.CityName = FromCity;

                var FromDistrict = getDistrict(order[i].FromDistrictId);
                order[i].FromDistrict = new District();
                order[i].FromDistrict.DistrictName = FromDistrict;

                var ToCountry = getCountry(order[i].ToCountryId);
                order[i].ToCountry = new Country();
                order[i].ToCountry.CountryName = ToCountry;

                var ToCity = getCity(order[i].ToCityId);
                order[i].ToCity = new City();
                order[i].ToCity.CityName = ToCity;

                var ToDistrict = getDistrict(order[i].ToDistrictId);
                order[i].ToDistrict = new District();
                order[i].ToDistrict.DistrictName = ToDistrict;
            }
            return order;
        }

        public JsonResult OnGetCities(int countryId)
        {
            Cities = (from city in _context.Cities
                      where city.CountryId == countryId
                      select new SelectListItem
                      {
                          Value = city.Id.ToString(),
                          Text = city.CityName
                      }).ToList();

            return new JsonResult(Cities);
        }

        public JsonResult OnGetDistrict(int cityId)
        {
            District = (from district in _context.District
                        where district.CityId == cityId
                        select new SelectListItem
                        {
                            Value = district.Id.ToString(),
                            Text = district.DistrictName
                        }).ToList();

            return new JsonResult(District);
        }

        public IActionResult OnPostDeliveredApproved([FromBody] UpdateDeliveredApproved model)
        {
            var product = _context.tbl_products.FirstOrDefault(x => x.ProductId == model.productId);
            var info = _context.MessageInfo.FirstOrDefault(x => x.productId == model.productId);
            var user = _context.AspNetUsers.FirstOrDefault(x => x.Id == product.TransporterUserId);
            if (product != null)
            {
                user.PaidPrice += product.Amount;
                product.isDelivered = 1;

                int deliverNumber = int.Parse(user.DeliverNumber);
                deliverNumber++;
                user.DeliverNumber = deliverNumber.ToString();

                int transportNumber = int.Parse(user.TransportNumber);
                transportNumber--;
                user.TransportNumber = transportNumber.ToString();

                info.isCompleted = 1;

                _context.SaveChanges();
                UserOrders = GetUserOrders(product.UserId);
                return new JsonResult(UserOrders);
            }
            else
            {
                return NotFound();
            }
        }

        public IActionResult OnPostUpdateProduct([FromBody] UpdateProductModel model)
        {
            var order = _context.tbl_products.FirstOrDefault(x => x.ProductId == model.productId);
            if (order != null)
            {
                order.OrderStatus = model.orderStatus;
                _context.SaveChanges();
                ProductTransporter = GetProductTransporter(order.TransporterUserId);
                return new JsonResult(ProductTransporter);
            }
            else
            {
                return NotFound(); // Ürün bulunamadý durumunda 404 döndürülebilir.
            }
        }

        public IActionResult OnGetComment(int productId)
        {
            var comment = _context.Comments.FirstOrDefault(x => x.ProductId == productId);

            if (comment != null)
            {
                var commentData = new
                {
                    Rating = comment.Rating,
                    CommentText = comment.CommentText,
                    CommentId = comment.CommentId
                };

                return new JsonResult(commentData);
            }
            else
            {
                return new JsonResult(null);
            }
        }


        public class UpdateProductModel
        {
            public int productId { get; set; }
            public string orderStatus { get; set; }
        }

        public class UpdateDeliveredApproved
        {
            public int productId { get; set; }
        }

        public IActionResult OnPostDeleteAdvert(int id)
        {
            var order = _context.tbl_products.FirstOrDefault(p => p.ProductId == id);

            if (order != null)
            {
                _context.tbl_products.Remove(order);
            }

            _context.SaveChanges();

            UserOrders = GetUserOrders(order.UserId);

            return new JsonResult(UserOrders);
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

    public class OrderFormData
    {
        public int ProductId { get; set; }
        public int FromCountryId { get; set; }
        public int FromCityId { get; set; }
        public int FromDistrictId { get; set; }
        public int ToCountryId { get; set; }
        public int ToCityId { get; set; }
        public int ToDistrictId { get; set; }
        public string ProductName { get; set; }
        public string Address { get; set; }
        public string ProductKg { get; set; }
        public string ProductNote { get; set; }
        public string WhenDate { get; set; }
        public byte[]? ProductImage { get; set; }
    }
}
