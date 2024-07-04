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
    public class MyAdvertModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly DbContextSwiftShift _context;

        public List<SelectListItem> Cities { get; set; }
        public List<SelectListItem> District { get; set; }

        public List<Product> UserAdverts { get; set; }

        public MyAdvertModel(
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
            UserAdverts = GetUserAdverts(userId);

            return Page();
        }

        public JsonResult OnGetAjaxMethod(int id)
        {
            var selectedadvert = _context.tbl_products.FirstOrDefault(x => x.ProductId == id);

            var FromCountry = getCountry(selectedadvert.FromCountryId);
            selectedadvert.FromCountry = new Country();
            selectedadvert.FromCountry.CountryName = FromCountry;

            var FromCity = getCity(selectedadvert.FromCityId);
            selectedadvert.FromCity = new City();
            selectedadvert.FromCity.CityName = FromCity;

            var FromDistrict = getDistrict(selectedadvert.FromDistrictId);
            selectedadvert.FromDistrict = new District();
            selectedadvert.FromDistrict.DistrictName = FromDistrict;

            var ToCountry = getCountry(selectedadvert.ToCountryId);
            selectedadvert.ToCountry = new Country();
            selectedadvert.ToCountry.CountryName = ToCountry;

            var ToCity = getCity(selectedadvert.ToCityId);
            selectedadvert.ToCity = new City();
            selectedadvert.ToCity.CityName = ToCity;

            var ToDistrict = getDistrict(selectedadvert.ToDistrictId);
            selectedadvert.ToDistrict = new District();
            selectedadvert.ToDistrict.DistrictName = ToDistrict;

            return new JsonResult(selectedadvert);
        }

        public List<Product> GetUserAdverts(string userId)
        {
            var adverts = _context.tbl_products.Where(x => x.UserId == userId && x.WhenDate >= DateTime.Now).ToList();

            for (int i = 0; i < adverts.Count; i++)
            {
                var FromCountry = getCountry(adverts[i].FromCountryId);
                adverts[i].FromCountry = new Country();
                adverts[i].FromCountry.CountryName = FromCountry;

                var FromCity = getCity(adverts[i].FromCityId);
                adverts[i].FromCity = new City();
                adverts[i].FromCity.CityName = FromCity;

                var FromDistrict = getDistrict(adverts[i].FromDistrictId);
                adverts[i].FromDistrict = new District();
                adverts[i].FromDistrict.DistrictName = FromDistrict;

                var ToCountry = getCountry(adverts[i].ToCountryId);
                adverts[i].ToCountry = new Country();
                adverts[i].ToCountry.CountryName = ToCountry;

                var ToCity = getCity(adverts[i].ToCityId);
                adverts[i].ToCity = new City();
                adverts[i].ToCity.CityName = ToCity;

                var ToDistrict = getDistrict(adverts[i].ToDistrictId);
                adverts[i].ToDistrict = new District();
                adverts[i].ToDistrict.DistrictName = ToDistrict;
            }
            return adverts;
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

        public IActionResult OnPostUpdateAdvert([FromBody] AdvertFormData formData)
        {
            var advert = _context.tbl_products.FirstOrDefault(x => x.ProductId == formData.ProductId);

            advert.FromCountryId = formData.FromCountryId;
            advert.FromCityId = formData.FromCityId;
            advert.FromDistrictId = formData.FromDistrictId;
            advert.ToCountryId = formData.ToCountryId;
            advert.ToCityId = formData.ToCityId;
            advert.ToDistrictId = formData.ToDistrictId;
            advert.WhenDate = DateTime.Parse(formData.WhenDate);
            advert.ProductName = formData.ProductName;
            advert.Address = formData.Address;
            advert.ProductKg = formData.ProductKg;
            advert.ProductNote = formData.ProductNote;

            _context.SaveChanges();

            UserAdverts = GetUserAdverts(advert.UserId);

            return new JsonResult(UserAdverts);
        }

        public IActionResult OnPostDeleteAdvert(int id)
        {
            var advert = _context.tbl_products.FirstOrDefault(p => p.ProductId == id);
            string userId = _userManager.GetUserId(User);
            var user = _context.AspNetUsers.FirstOrDefault(x => x.Id == userId);

            if (advert != null)
            {
                _context.tbl_products.Remove(advert);
            }

            int advertNumber = int.Parse(user.AdvertNumber);
            advertNumber--;
            user.AdvertNumber = advertNumber.ToString();

            _context.SaveChanges();
            UserAdverts = GetUserAdverts(userId);

            return new JsonResult(UserAdverts);
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

    public class AdvertFormData
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
