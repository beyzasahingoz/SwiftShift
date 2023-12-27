#nullable disable
using Bitirme.Areas.Identity.Data;
using Bitirme.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Bitirme.Views.Main
{
    public class AdvertModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DbContextSwiftShift _context;

        public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> Products { get; set; }
        public List<SelectListItem> Countries { get; set; }
        public List<SelectListItem> Cities { get; set; }
        public AdvertModel(
             UserManager<ApplicationUser> userManager,
            DbContextSwiftShift context
            )
        {
            _userManager = userManager;
            _context = context;
            Cities = GetCities();
            Countries = GetCountries();
            Products = GetProducts();
        }

        //public async Task<IActionResult> OnGetAsync()
        //{
        //    //var user = await _userManager.GetUserAsync(User);
        //    //if (user == null)
        //    //{
        //    //    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        //    //}

        //    //await LoadAsync(user);

        //    //return Page();
        //}
        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {
            [Display(Name = "Ürün Adý")]
            public string? ProductName { get; set; }

            [Display(Name = "Adres")]
            public string Address { get; set; }

            [Display(Name = "Kilogram")]
            public string ProductKg { get; set; }

            [Display(Name = "Not")]
            public string ProductNote { get; set; }

            [Display(Name = "Ülke")]
            public string Country { get; set; }

            [Display(Name = "Ürün")]
            public string Product { get; set; }

            [Display(Name = "Þehir")]
            public string City { get; set; }
        }
        //private ApplicationUser CreateProduct()
        //{
        //    try
        //    {
        //        return Activator.CreateInstance<Product>();
        //    }
        //    catch
        //    {
        //        throw new InvalidOperationException($"Can't create an instance of '{nameof(Product)}'. " +
        //            $"Ensure that '{nameof(Product)}' is not an abstract class and has a parameterless constructor, or alternatively " +
        //            $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
        //    }
        //}
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                //    var user = CreateProduct();
                var productName = Input.ProductName;
                var productNote = Input.ProductNote;
                var productKg = Input.ProductKg;
                //var address = Input.Address;
                //var country = getCountry(user.CountryId);
                //var city = getCity(user.CityId);               
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
        //private async Task LoadAsync(ApplicationUser user)
        //{
        //    var productName = user.Product.ProductName;
        //    var productNote = user.Product.ProductNote;
        //    var productKg = user.Product.ProductKg;
        //    var address = user.Product.Address;
        //    var country = user.Country.CountryName;
        //    var product = getProduct((int)user.Product.ProductId);
        //    var city = getCity(user.CityId);
        //    Input = new InputModel
        //    {
        //        City = city,
        //        Country = country,
        //        Product = product,
        //        ProductKg = productKg,
        //        ProductNote = productNote,
        //        ProductName = productName,
        //        Address = address
        //    };
        //    Products = GetProducts((int)user.Product.ProductId);
        //    Countries = GetCountries(user.CountryId);
        //    Cities = GetCities(user.CountryId, user.CityId);
        //}
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
        private string getProduct(int productId)
        {
            return _context.tbl_products
                 .Where(c => c.ProductId == productId)
                 .OrderBy(n => n.ProductName)
                 .Select(c => c.ProductName).ToList()[0];
        }


        private List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetProducts(int productId = 1)
        {
            var lstProducts = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            List<Product> products = _context.tbl_products.ToList();

            lstProducts = products.Select(pr => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
            {
                Value = pr.ProductId.ToString(),
                Text = pr.ProductName,
            }).ToList();

            var defItem = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
            {
                Value = "",
                Text = ""
            };

            lstProducts.Insert(0, defItem);
            return lstProducts;
        }
    }
}
