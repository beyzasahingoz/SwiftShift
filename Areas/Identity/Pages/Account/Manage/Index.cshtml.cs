// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bitirme.Areas.Identity.Data;
using Bitirme.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitirme.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly DbContextSwiftShift _context;

        public CascadingModel CascadingModel { get; set; }

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
            CascadingModel = new CascadingModel();
            CascadingModel.Countries = GetCountries();
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
            [Display(Name = "Telefon Numarası")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Profil Fotoğrafı")]
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

            [Display(Name = "Adres")]
            public string Address { get; set; }

            [Display(Name = "TC Kimlik Numarası")]
            public string TCKN { get; set; }

            [Display(Name = "Doğum Tarihi")]
            [DataType(DataType.Date)]
            public DateTime BirthDate { get; set; }

            [Display(Name = "isVerified")]
            public int isVerified { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;
            var profilePicture = user.ProfilePicture;
            var firstName = user.Ad;
            var lastName = user.Soyad;
            var country = getCountry(user.CountryId);
            var city = getCity(user.CityId);
            var district = getDistrict(user.DistrictId);
            var address = user.Address;
            var tckn = user.TCKN;
            var birthDate = user.BirthDate;
            var isVerified = user.isVerified;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                ProfilePicture = profilePicture,
                FirstName = firstName,
                LastName = lastName,
                Country = country,
                City = city,
                District = district,
                Address = address,
                TCKN = tckn,
                BirthDate = birthDate,
                isVerified = isVerified
            };
            CascadingModel.Cities = GetCities(user.CountryId);
            CascadingModel.District = GetDistrict(user.CityId);
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
            var client = new MernisService.KPSPublicSoapClient(MernisService.KPSPublicSoapClient.EndpointConfiguration.KPSPublicSoap);

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
            bool isUpdate = false;
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                string RegexDesen = @"^(0(\d{3}) (\d{3}) (\d{2}) (\d{2}))$";
                Match Eslesme = Regex.Match(Input.PhoneNumber, RegexDesen, RegexOptions.IgnoreCase);

                if (!Eslesme.Success)
                {
                    StatusMessage = "Telefon Numarasını TR Formatına Uygun Biçimde Giriniz.";
                    return RedirectToPage();
                }
                user.PhoneNumber = Input.PhoneNumber;
                isUpdate = true;
            }

            if (Input.FirstName != user.Ad)
            {
                user.Ad = Input.FirstName;
                isUpdate = true;
            }

            if (Input.LastName != user.Soyad)
            {
                user.Soyad = Input.LastName;
                isUpdate = true;
            }
            if (Input.Address != user.Address)
            {
                user.Address = Input.Address;
                isUpdate = true;
            }
            if (Input.BirthDate != user.BirthDate)
            {
                user.BirthDate = Input.BirthDate;
                isUpdate = true;
            }

            if (Input.TCKN != null)
            {
                var response = await client.TCKimlikNoDogrulaAsync(long.Parse(Input.TCKN), Input.FirstName, Input.LastName, Input.BirthDate.Year);
                var result = response.Body.TCKimlikNoDogrulaResult;

                if (result)
                {
                    user.TCKN = Input.TCKN;
                    user.isVerified = 1;
                    isUpdate = true;
                }
                else
                {
                    TempData["SweetAlertMessage"] = " ";
                    return RedirectToPage();
                }
            }

            bool isChangeLocation = false;

            if (Input.Country != null)
            {
                if (Int32.Parse(Input.Country) != user.CountryId)
                {
                    user.CountryId = Int32.Parse(Input.Country);
                    isChangeLocation = true;
                }
            }

            if (Input.City != null)
            {
                if (Int32.Parse(Input.City) != user.CityId)
                {
                    user.CityId = Int32.Parse(Input.City);
                    isChangeLocation = true;
                }
            }

            if (Input.District != null)
            {
                if (Int32.Parse(Input.District) != user.DistrictId)
                {
                    user.DistrictId = Int32.Parse(Input.District);
                    isChangeLocation = true;
                }
            }

            if (isChangeLocation)
            {
                string country = getCountry(user.CountryId);
                string city = getCity(user.CityId);
                string district = getDistrict(user.DistrictId);

                Coordinates coordinates = GetCoordinates(country, city, district).Result;
                user.Latitude = coordinates.Latitude;
                user.Longitude = coordinates.Longitude;

                isUpdate = true;
            }

            if (Request.Form.Files.Count > 0)
            {
                IFormFile file = Request.Form.Files.FirstOrDefault();
                using (var dataStream = new MemoryStream())
                {
                    await file.CopyToAsync(dataStream);
                    user.ProfilePicture = dataStream.ToArray();
                }
                isUpdate = true;
            }

            if (isUpdate)
            {
                await _userManager.UpdateAsync(user);
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Profiliniz Başarılı Bir Şekilde Güncellenmiştir.";
            return RedirectToPage();
        }

        #region country city district
        private List<SelectListItem> GetCountries()
        {
            Countries = (from country in _context.Countries
                         select new SelectListItem
                         {
                             Value = country.Id.ToString(),
                             Text = country.CountryName
                         }).ToList();

            return Countries;
        }

        private List<SelectListItem> GetCities(int countryId = 1)
        {
            Cities = (from city in _context.Cities
                      where city.CountryId == countryId
                      select new SelectListItem
                      {
                          Value = city.Id.ToString(),
                          Text = city.CityName
                      }).ToList();

            return Cities;
        }

        private List<SelectListItem> GetDistrict(int cityId = 1)
        {
            District = (from district in _context.District
                        where district.CityId == cityId
                        select new SelectListItem
                        {
                            Value = district.Id.ToString(),
                            Text = district.DistrictName
                        }).ToList();

            return District;
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

        #endregion

        public IActionResult OnPostAjaxMethod(string type, int value)
        {
            CascadingModel = new CascadingModel();
            switch (type)
            {
                case "ddlCountries":
                    CascadingModel.Cities = (from city in _context.Cities
                                             where city.CountryId == value
                                             select new SelectListItem
                                             {
                                                 Value = city.Id.ToString(),
                                                 Text = city.CityName
                                             }).ToList();
                    break;
                case "ddlCities":
                    CascadingModel.District = (from district in _context.District
                                               where district.CityId == value
                                               select new SelectListItem
                                               {
                                                   Value = district.Id.ToString(),
                                                   Text = district.DistrictName
                                               }).ToList();
                    break;
            }
            return new JsonResult(CascadingModel);
        }
        private async Task<Coordinates> ReCalculateLatLng(string lat, string lng)
        {
            var R = 6378137.0;
            Random rand = new();

            var DistanceNorth = rand.Next(3, 5);
            var DistanceEast = rand.Next(3, 5);

            var dLat = DistanceNorth / R;
            var dLon = DistanceEast / (R * Math.Cos(Math.PI * Convert.ToDouble(lat) / 180));

            var NewLat = Convert.ToDouble(lat) + dLat * 180 / Math.PI;
            var NewLng = Convert.ToDouble(lng) + dLon * 180 / Math.PI;

            return new Coordinates { Latitude = (NewLat.ToString()).Replace(',', '.'), Longitude = (NewLng.ToString()).Replace(',', '.') };
        }
        private async Task<Coordinates> GetCoordinates(string country, string city, string district)
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
                            lat = (Convert.ToString(item.geometry.location.lat)).Replace(',', '.');
                            longi = (Convert.ToString(item.geometry.location.lng)).Replace(',', '.');
                        }
                        var isExist = await _context.AspNetUsers
                            .AnyAsync(x => x.Latitude.Equals(lat) && x.Longitude.Equals(longi));

                        if (!isExist)
                        {
                            return new Coordinates { Latitude = lat, Longitude = longi };
                        }
                        else
                        {
                            while (isExist)
                            {
                                Coordinates coordinates = await ReCalculateLatLng(lat, longi);
                                lat = coordinates.Latitude;
                                longi = coordinates.Longitude;

                                isExist = await _context.AspNetUsers
                                    .AnyAsync(x => x.Latitude.Equals(lat) && x.Longitude.Equals(longi));

                                if (!isExist)
                                {
                                    return new Coordinates { Latitude = lat, Longitude = longi };
                                }
                            }

                        }
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
