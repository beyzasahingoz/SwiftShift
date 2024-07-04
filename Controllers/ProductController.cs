using Bitirme.Areas.Identity.Data;
using Bitirme.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using System.Web;

namespace Bitirme.Controllers
{
    public class ProductController : Controller
    {
        private DbContextSwiftShift _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CascadingModel CascadingFromModel { get; set; }
        public List<SelectListItem> FromCountries { get; set; }

        public CascadingModel CascadingToModel { get; set; }
        public List<SelectListItem> ToCountries { get; set; }

        public List<Travel> Travels { get; set; }

        public ProductController(
            DbContextSwiftShift context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

            CascadingModel CascadingFromModel = new CascadingModel();
            CascadingFromModel.Countries = GetFromCountries();

            CascadingModel CascadingToModel = new CascadingModel();
            CascadingToModel.Countries = GetToCountries();
        }

        [HttpGet]
        public IActionResult Advert()
        {
            ViewBag.FromCountries = GetFromCountries();
            ViewBag.ToCountries = GetToCountries();
            return View();
        }

        public IActionResult Explore()
        {
            string userId = _userManager.GetUserId(User);

            var products = _context.tbl_products.Where(x => x.UserId != userId && x.isOrder == 0 && x.WhenDate >= DateTime.Now).ToList();
            var travels = GetTravels(userId);

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

            var combinedModel = new CombinedExploreModel
            {
                Products = products,
                Travels = travels
            };
            return View(combinedModel);
        }
        public IActionResult MyOrder()
        {
            string userId = _userManager.GetUserId(User);

            var products = _context.tbl_products.Where(x => x.UserId != userId).ToList();
            return View(products);
        }

        private List<SelectListItem> GetFromCountries()
        {
            FromCountries = (from country in _context.Countries
                             select new SelectListItem
                             {
                                 Value = country.Id.ToString(),
                                 Text = country.CountryName
                             }).ToList();

            return FromCountries;
        }

        private List<SelectListItem> GetToCountries()
        {
            ToCountries = (from country in _context.Countries
                           select new SelectListItem
                           {
                               Value = country.Id.ToString(),
                               Text = country.CountryName
                           }).ToList();

            return ToCountries;
        }

        [HttpPost]
        public IActionResult AjaxMethod(string type, int value)
        {
            if (type.Contains("From"))
            {
                CascadingFromModel = new CascadingModel();
                switch (type)
                {
                    case "ddlFromCountries":
                        CascadingFromModel.Cities = (from city in _context.Cities
                                                     where city.CountryId == value
                                                     select new SelectListItem
                                                     {
                                                         Value = city.Id.ToString(),
                                                         Text = city.CityName
                                                     }).ToList();
                        ViewBag.FromCities = CascadingFromModel.Cities;
                        break;
                    case "ddlFromCities":
                        CascadingFromModel.District = (from district in _context.District
                                                       where district.CityId == value
                                                       select new SelectListItem
                                                       {
                                                           Value = district.Id.ToString(),
                                                           Text = district.DistrictName
                                                       }).ToList();
                        ViewBag.FromDistrict = CascadingFromModel.District;
                        break;
                }
                return new JsonResult(CascadingFromModel);
            }
            else
            {
                CascadingToModel = new CascadingModel();
                switch (type)
                {
                    case "ddlToCountries":
                        CascadingToModel.Cities = (from city in _context.Cities
                                                   where city.CountryId == value
                                                   select new SelectListItem
                                                   {
                                                       Value = city.Id.ToString(),
                                                       Text = city.CityName
                                                   }).ToList();
                        ViewBag.ToCities = CascadingToModel.Cities;
                        break;
                    case "ddlToCities":
                        CascadingToModel.District = (from district in _context.District
                                                     where district.CityId == value
                                                     select new SelectListItem
                                                     {
                                                         Value = district.Id.ToString(),
                                                         Text = district.DistrictName
                                                     }).ToList();
                        ViewBag.ToDistrict = CascadingToModel.District;
                        break;
                }
                return new JsonResult(CascadingToModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Advert(Product model)
        {
            string userId = _userManager.GetUserId(User);
            var user = _context.AspNetUsers.FirstOrDefault(x => x.Id == userId);

            if (!ModelState.IsValid)
            {
                var product = new Product
                {
                    ProductName = model.ProductName,
                    Address = model.Address,
                    ProductKg = model.ProductKg,
                    ProductNote = model.ProductNote,
                    FromCountryId = model.FromCountryId,
                    FromCityId = model.FromCityId,
                    FromDistrictId = model.FromDistrictId,
                    ToCountryId = model.ToCountryId,
                    ToCityId = model.ToCityId,
                    ToDistrictId = model.ToDistrictId,
                    //ProductImage = model.ProductImage,
                    WhenDate = model.WhenDate,
                    UserId = _userManager.GetUserId(User),
                    isOrder = 0,
                    OrderStatus = "0",
                    TransporterUserId = "0"
                };

                int advertNumber = int.Parse(user.AdvertNumber);
                advertNumber++;
                user.AdvertNumber = advertNumber.ToString();

                if (Request.Form.Files.Count > 0 && Request.Form.Files["ProductImage"] != null && Request.Form.Files["ProductImage"].Length > 0)
                {
                    IFormFile file = Request.Form.Files["ProductImage"];
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        product.ProductImage = memoryStream.ToArray();
                    }
                }
                else
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        var defaultImagePath = "wwwroot/images/Logo.png";
                        var defaultImageBytes = System.IO.File.ReadAllBytes(defaultImagePath);
                        await memoryStream.WriteAsync(defaultImageBytes, 0, defaultImageBytes.Length);
                        product.ProductImage = memoryStream.ToArray();
                    }
                }


                _context.tbl_products.Add(product);
                _context.SaveChanges();

                TempData["SweetAlertMessage"] = " ";

                return RedirectToAction("Advert");
            }

            return View(model);
        }

        [HttpPost]
        public string PostUsingRequest()
        {
            string productName = Request.Form["productName"].ToString();
            string address = Request.Form["address"].ToString();
            string productKg = Request.Form["productKg"].ToString();
            string productNote = Request.Form["productNote"].ToString();

            return "From parameters - " + productName + "," + address + "," + productKg + "," + productNote;
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

        public List<Travel> GetTravels(string userId)
        {
            var travels = _context.Travel.Where(x => x.DepartureDate >= DateTime.Now && x.UserId != userId).ToList();

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
            return travels;
        }
    }
}