using Bitirme.Areas.Identity.Data;
using Bitirme.Models;
using Microsoft.AspNetCore.Mvc;
using System.Web.WebPages.Html;

namespace Bitirme.Controllers
{
    public class ProductController : Controller
    {
        private DbContextSwiftShift _context;

        public ProductController(
            DbContextSwiftShift context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Advert()
        {
            ViewBag.Countries = GetCountries();
            ViewBag.Cities = GetCities();
            return View();
        }
        private List<SelectListItem> GetCities(int countryId = 1, int cityId = 1)
        {
            List<SelectListItem> lstCities = _context.Cities
                .Where(c => c.CountryId == countryId)
                .OrderBy(n => n.CityName)
                .Select(n =>
                new SelectListItem
                {
                    Value = n.Id.ToString(),
                    Text = n.CityName
                }).ToList();

            var defItem = new SelectListItem()
            {
                Value = "",
                Text = "----Select City----"
            };

            lstCities.Insert(0, defItem);
            return lstCities;
        }
        private List<SelectListItem> GetCountries(int countryId = 1)
        {
            var lstCountries = new List<SelectListItem>();

            List<Country> Countries = _context.Countries.ToList();

            lstCountries = Countries.Select(ct => new SelectListItem()
            {
                Value = ct.Id.ToString(),
                Text = ct.CountryName
            }).ToList();

            var defItem = new SelectListItem()
            {
                Value = "",
                Text = "----Select Country----"
            };

            lstCountries.Insert(0, defItem);
            return lstCountries;
        }
        [HttpPost]
        public IActionResult Advert(Product model)
        {

            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    ProductName = model.ProductName,
                    Address = model.Address,
                    ProductKg = model.ProductKg,
                    ProductNote = model.ProductNote,
                    CityId = model.CityId,
                    CountryId = model.CountryId
                };

                _context.tbl_products.Add(product);
                _context.SaveChanges();

                return RedirectToAction("Advert");
            }

            ViewBag.Countries = GetCountries();
            ViewBag.Cities = GetCities();
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
    }
}