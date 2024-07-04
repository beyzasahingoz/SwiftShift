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
    public class MyTravelModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly DbContextSwiftShift _context;

        public List<SelectListItem> Countries { get; set; }
        public List<SelectListItem> Cities { get; set; }
        public List<SelectListItem> District { get; set; }

        public List<Travel> UserTravels { get; set; }

        public MyTravelModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            DbContextSwiftShift context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public string Username { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userId = await _userManager.GetUserIdAsync(user);
            UserTravels = GetUserTravels(userId);

            return Page();
        }

        public JsonResult OnGetAjaxMethod(int id)
        {
            var selectedtravel = _context.Travel.Where(x => x.TravelId == id).FirstOrDefault();

            var DepartureCountry = getCountry(selectedtravel.DepartureCountryId);
            selectedtravel.DepartureCountry = new Country();
            selectedtravel.DepartureCountry.CountryName = DepartureCountry;

            var DepartureCity = getCity(selectedtravel.DepartureCityId);
            selectedtravel.DepartureCity = new City();
            selectedtravel.DepartureCity.CityName = DepartureCity;

            var DepartureDistrict = getDistrict(selectedtravel.DepartureDistrictId);
            selectedtravel.DepartureDistrict = new District();
            selectedtravel.DepartureDistrict.DistrictName = DepartureDistrict;

            var ArrivalCountry = getCountry(selectedtravel.ArrivalCountryId);
            selectedtravel.ArrivalCountry = new Country();
            selectedtravel.ArrivalCountry.CountryName = ArrivalCountry;

            var ArrivalCity = getCity(selectedtravel.ArrivalCityId);
            selectedtravel.ArrivalCity = new City();
            selectedtravel.ArrivalCity.CityName = ArrivalCity;

            var ArrivalDistrict = getDistrict(selectedtravel.ArrivalDistrictId);
            selectedtravel.ArrivalDistrict = new District();
            selectedtravel.ArrivalDistrict.DistrictName = ArrivalDistrict;

            return new JsonResult(selectedtravel);
        }

        public List<Travel> GetUserTravels(string userId)
        {
            var travels = _context.Travel.Where(x => x.UserId == userId && x.DepartureDate >= DateTime.Now).ToList();

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

        public IActionResult OnPostUpdateTravel([FromBody] TravelFormData formData)
        {
            var travel = _context.Travel.FirstOrDefault(t => t.TravelId == formData.TravelId);

            travel.DepartureCountryId = formData.DepartureCountryId;
            travel.DepartureCityId = formData.DepartureCityId;
            travel.DepartureDistrictId = formData.DepartureDistrictId;
            travel.ArrivalCountryId = formData.ArrivalCountryId;
            travel.ArrivalCityId = formData.ArrivalCityId;
            travel.ArrivalDistrictId = formData.ArrivalDistrictId;
            travel.ArrivalDate = DateTime.Parse(formData.ArrivalDate); 
            travel.DepartureDate = DateTime.Parse(formData.DepartureDate);
            travel.MaxWeightCarry = formData.MaxWeightCarry;
            travel.pricePerKg = formData.PricePerKg;
            travel.seeProduct = formData.SeeProduct;
            travel.carrySensitiveProduct = formData.CarrySensitiveProduct;

            _context.SaveChanges();

            UserTravels = GetUserTravels(travel.UserId);

            return new JsonResult(UserTravels);
        }

        public IActionResult OnPostDeleteTravel(int id)
        {
            var travel = _context.Travel.FirstOrDefault(t => t.TravelId == id);

            if(travel != null)
            {
                _context.Travel.Remove(travel);
            }

            _context.SaveChanges();

            UserTravels = GetUserTravels(travel.UserId);

            return new JsonResult(UserTravels);
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

    public class TravelFormData
    {
        public int TravelId { get; set; }
        public int DepartureCountryId { get; set; }
        public int DepartureCityId { get; set; }
        public int DepartureDistrictId { get; set; }
        public int ArrivalCountryId { get; set; }
        public int ArrivalCityId { get; set; }
        public int ArrivalDistrictId { get; set; }
        public string ArrivalDate { get; set; }
        public string DepartureDate { get; set; }
        public int MaxWeightCarry { get; set; }
        public double PricePerKg { get; set; }
        public int SeeProduct { get; set; }
        public int CarrySensitiveProduct { get; set; }
    }
}
