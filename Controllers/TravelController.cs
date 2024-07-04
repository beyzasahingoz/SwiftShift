using Bitirme.Areas.Identity.Data;
using Bitirme.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using Microsoft.AspNetCore.Identity;

namespace Bitirme.Controllers
{
    public class TravelController : Controller
    {
        private DbContextSwiftShift _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CascadingModel CascadingDepartureModel { get; set; }
        public List<SelectListItem> DepartureCountries { get; set; }

        public CascadingModel CascadingArrivalModel { get; set; }
        public List<SelectListItem> ArrivalCountries { get; set; }

        public TravelController(
            DbContextSwiftShift context, UserManager<ApplicationUser> userManager)
        {
            _context = context;

            CascadingModel CascadingDepartureModel = new CascadingModel();
            CascadingDepartureModel.Countries = GetDepartureCountries();

            CascadingModel CascadingArrivalModel = new CascadingModel();
            CascadingArrivalModel.Countries = GetArrivalCountries();
            _userManager = userManager;
        }
        [System.Web.Mvc.HttpGet]
        public IActionResult AddTravel()
        {
            ViewBag.DepartureCountries = GetDepartureCountries();
            ViewBag.ArrivalCountries = GetArrivalCountries();
            return View();
        }
        private List<SelectListItem> GetDepartureCountries()
        {
            DepartureCountries = (from country in _context.Countries
                                  select new SelectListItem
                                  {
                                      Value = country.Id.ToString(),
                                      Text = country.CountryName
                                  }).ToList();

            return DepartureCountries;
        }

        private List<SelectListItem> GetArrivalCountries()
        {
            ArrivalCountries = (from country in _context.Countries
                                select new SelectListItem
                                {
                                    Value = country.Id.ToString(),
                                    Text = country.CountryName
                                }).ToList();

            return ArrivalCountries;
        }

        [HttpPost]
        public IActionResult AjaxMethod(string type, int value)
        {
            if (type.Contains("Departure"))
            {
                CascadingDepartureModel = new CascadingModel();
                switch (type)
                {
                    case "ddlDepartureCountries":
                        CascadingDepartureModel.Cities = (from city in _context.Cities
                                                          where city.CountryId == value
                                                          select new SelectListItem
                                                          {
                                                              Value = city.Id.ToString(),
                                                              Text = city.CityName
                                                          }).ToList();
                        ViewBag.DepartureCities = CascadingDepartureModel.Cities;
                        break;
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
                    case "ddlArrivalCountries":
                        CascadingArrivalModel.Cities = (from city in _context.Cities
                                                        where city.CountryId == value
                                                        select new SelectListItem
                                                        {
                                                            Value = city.Id.ToString(),
                                                            Text = city.CityName
                                                        }).ToList();
                        ViewBag.ArrivalCities = CascadingArrivalModel.Cities;
                        break;
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

        [HttpPost]
        public IActionResult AddTravel(Travel model)
        {
            string userId = _userManager.GetUserId(User);
            var user = _context.AspNetUsers.FirstOrDefault(x => x.Id == userId);

            if (user.isVerified == 0)
            {
                TempData["NotVerifiedMessage"] = " ";
                return Redirect("~/Identity/Account/Manage"); ;
            }
            if (!ModelState.IsValid)
            {
                var travel = new Travel
                {
                    ArrivalCityId = model.ArrivalCityId,
                    ArrivalCountryId = model.ArrivalCountryId,
                    ArrivalDistrictId = model.ArrivalDistrictId,
                    ArrivalDate = model.ArrivalDate,
                    DepartureCityId = model.DepartureCityId,
                    DepartureCountryId = model.DepartureCountryId,
                    DepartureDistrictId = model.DepartureDistrictId,
                    DepartureDate = model.DepartureDate,
                    MaxWeightCarry = model.MaxWeightCarry,
                    pricePerKg = model.pricePerKg,
                    UserId = _userManager.GetUserId(User),
                    seeProduct = model.seeProduct == 1 ? 1 : 0,
                    carrySensitiveProduct = model.carrySensitiveProduct == 1 ? 1 : 0
                };
                _context.Travel.Add(travel);
                _context.SaveChanges();

                TempData["SweetAlertMessage"] = " ";

                return RedirectToAction("AddTravel");
            }
            return View(model);
        }
    }
}
