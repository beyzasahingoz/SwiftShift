using Bitirme.Areas.Identity.Data;
using Bitirme.Models;
using Bitirme.Views.Main;
using Microsoft.AspNetCore.Mvc;

namespace Bitirme.Controllers
{
    public class ExploreController : Controller
    {
        private readonly DbContextSwiftShift _context;

        public ExploreController(DbContextSwiftShift context)
        {
            _context = context;
        }

        public IActionResult Explore()
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
                    Note = product.ProductNote,
                    ProductImage = product.ProductImage
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
    }
}
