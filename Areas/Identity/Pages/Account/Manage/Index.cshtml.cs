// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Bitirme.Areas.Identity.Data;
using Bitirme.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Bitirme.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly DbContextSwiftShift _context;

        public List<SelectListItem> Countries { get; set; }
        public List<SelectListItem> Cities { get; set; }
        public List<SelectListItem> District { get; set; }
        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            DbContextSwiftShift context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            Countries = GetCountries();
            Cities = GetCities();
            District = GetDistrict();
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Profile Picture")]
            public byte[] ProfilePicture { get; set; }

            [Display(Name = "Ad")]
            public string FirstName { get; set; }

            [Display(Name = "Soyad")]
            public string LastName { get; set; }

            [Display(Name = "Ülke")]
            public string Country { get; set; }

            [Display(Name = "Şehir")]
            public string City { get; set; }

            [Display(Name = "İlçe")]
            public string District { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;
            var profilePicture = user.ProfilePicture;
            var firstName = user.Ad;
            var lastName = user.Soyad;
            var country = user.Country.CountryName;
            var city = getCity(user.CityId);
            var district = getDistrict(user.DistrictId);    

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                ProfilePicture = profilePicture,
                FirstName = firstName,
                LastName = lastName,
                Country = country,
                City = city,
                District = district
            };
            Countries = GetCountries(user.CountryId);
            Cities = GetCities(user.CountryId, user.CityId);
            District = GetDistrict(user.CityId, user.DistrictId);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }
            
            if (Input.FirstName != user.Ad)
            {
                user.Ad = Input.FirstName;
                await _userManager.UpdateAsync(user);
            }

            if (Input.LastName != user.Soyad)
            {
                user.Soyad = Input.LastName;
                await _userManager.UpdateAsync(user);
            }

            bool isChangeLocation = false;

            if (Int32.Parse(Input.Country) != user.CountryId)
            {
                user.CountryId = Int32.Parse(Input.Country);
                isChangeLocation = true;
            }

            if (Int32.Parse(Input.City) != user.CityId)
            {
                user.CityId = Int32.Parse(Input.City);
                isChangeLocation = true;
            }

            if (Int32.Parse(Input.District) != user.DistrictId)
            {
                user.DistrictId = Int32.Parse(Input.District);
                isChangeLocation = true;
            }

            if (isChangeLocation)
            {
                string country = getCountry(user.CountryId);
                string city = getCity(user.CityId);
                string district = getDistrict(user.DistrictId);

                Coordinates coordinates = GetCoordinates(country, city, district).Result;
                user.Latitude = coordinates.Latitude;
                user.Longitude = coordinates.Longitude;

                await _userManager.UpdateAsync(user);
            }

            if (Request.Form.Files.Count > 0)
            {
                IFormFile file = Request.Form.Files.FirstOrDefault();
                using (var dataStream = new MemoryStream())
                {
                    await file.CopyToAsync(dataStream);
                    user.ProfilePicture = dataStream.ToArray();
                }
                await _userManager.UpdateAsync(user);
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
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
                Value = countryId.ToString(),
                Text = _context.Countries
                .Where(c => c.Id == countryId)
                .OrderBy(n => n.CountryName)
                .Select(n => n.CountryName).FirstOrDefault()
            };

            lstCountries.Insert(0, defItem);
            return lstCountries;
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
                Value = cityId.ToString(),
                Text = _context.Cities
                .Where(c => c.Id == cityId)
                .OrderBy(n => n.CityName)
                .Select(n => n.CityName).FirstOrDefault()
            };

            lstCities.Insert(0, defItem);
            return lstCities;
        }

        private List<SelectListItem> GetDistrict(int cityId = 1, int districtId = 1)
        {
            List<SelectListItem> lstDistrict = _context.District
                .Where(c => c.CityId == cityId)
                .OrderBy(n => n.DistrictName)
                .Select(n =>
                new SelectListItem
                {
                    Value = n.Id.ToString(),
                    Text = n.DistrictName
                }).ToList();

            var defItem = new SelectListItem()
            {
                Value = districtId.ToString(),
                Text = _context.District
                .Where(c => c.Id == districtId)
                .OrderBy(n => n.DistrictName)
                .Select(n => n.DistrictName).FirstOrDefault()
            };

            lstDistrict.Insert(0, defItem);

            return lstDistrict;
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
        public static async Task<Coordinates> GetCoordinates(string country, string city, string district)
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var address = $"{city}, {district}, {country}";
                    var apiUrl = $"https://maps.googleapis.com/maps/api/geocode/json?address={address}&key=AIzaSyC002asiPqu9JT7fbl_wAvmlV3uuZLwdvE";

                    HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<dynamic>(result);
                        string lat = "", longi = "";
                        foreach (var item in data.results)
                        {
                            lat = Convert.ToString(item.geometry.location.lat);
                            longi = Convert.ToString(item.geometry.location.lng);
                        }
                        return new Coordinates { Latitude = lat, Longitude = longi };

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }

                return null;
            }
        }
    }
}
