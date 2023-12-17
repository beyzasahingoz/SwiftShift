// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Bitirme.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Bitirme.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Microsoft.CodeAnalysis;
using Image = System.Drawing.Image;

namespace Bitirme.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly DbContextSwiftShift _context;
        public List<SelectListItem> Countries { get; }
        public List<SelectListItem> Cities { get; }
        public List<SelectListItem> District { get; }

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            DbContextSwiftShift context
        )
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
            Countries = GetCountries();
            Cities = GetCities();
            District = GetDistrict();
        }

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
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            [Required]
            [StringLength(255, ErrorMessage = "Max 255 characters are allowed")]
            [Display(Name = "Ad")]
            public string Ad { get; set; }

            [Required]
            [StringLength(255, ErrorMessage = "Max 255 characters are allowed")]
            [Display(Name = "Soyad")]
            public string Soyad { get; set; }
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            /// 

            [Required]
            [Display(Name = "Ülke")]
            public int CountryId { get; set; }

            [Required]
            [Display(Name = "Şehir")]
            public int CityId { get; set; }

            [Required]
            [Display(Name = "İlçe")]
            public int DistrictId { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                string country = getCountry(Input.CountryId);
                string city = getCity(Input.CityId);
                string district = getDistrict(Input.DistrictId);

                Coordinates coordinates = GetCoordinates(country, city, district).Result;

                user.Ad = Input.Ad;
                user.Soyad = Input.Soyad;
                user.ProfilePicture = ImageToByteArr();
                user.CountryId = Input.CountryId;
                user.CityId = Input.CityId;
                user.DistrictId = Input.DistrictId;
                user.Latitude = coordinates.Latitude;
                user.Longitude = coordinates.Longitude;

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }

        private byte[] ImageToByteArr()
        {
            Image image = Image.FromFile("C:\\Users\\beyza\\source\\repos\\SwiftShift\\wwwroot\\images\\profilePicture.png");
            using (var ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                return ms.ToArray();
            }
        }

        private List<SelectListItem> GetCountries()
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
        private string getDistrict(int districtId)
        {
            return _context.District
                 .Where(c => c.Id == districtId)
                 .OrderBy(n => n.DistrictName)
                 .Select(c => c.DistrictName).ToList()[0];
        }

        private List<SelectListItem> GetCities(int countryId = 1)
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

        private List<SelectListItem> GetDistrict(int cityId = 1)
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
                Value = "",
                Text = "----Select District----"
            };

            lstDistrict.Insert(0, defItem);

            return lstDistrict;
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

    public class Coordinates
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}

