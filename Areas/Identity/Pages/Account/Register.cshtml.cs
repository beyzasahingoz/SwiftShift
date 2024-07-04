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
using Microsoft.EntityFrameworkCore;
using System;
using System.Runtime.InteropServices;
using MimeKit;
using System.Net.Mail;
using MailKit.Net.Smtp;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

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

        public CascadingModel CascadingModel { get; set; }

        public List<SelectListItem> Countries { get; set; }


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
            CascadingModel = new CascadingModel();
            CascadingModel.Countries = GetCountries();
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
            [Display(Name = "Şifre")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Şifre Doğrulama")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "Cinsiyet")]
            public string Gender { get; set; }
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
                Random random = new Random();
                int code;
                code = random.Next(100000, 1000000);
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
                user.Point = 0;
                user.ProfileDescription = "";
                user.ConfirmCode = code;
                user.UserIBAN = "";
                user.TCKN = "";
                user.Gender = Input.Gender;
                user.RegisterDate = DateTime.Now;
                user.DeliverNumber = "0";
                user.AdvertNumber = "0";
                user.TransportNumber = "0";

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    MimeMessage mimeMessage = new MimeMessage();
                    MailboxAddress mailboxAddressFrom = new MailboxAddress("SwiftShift Admin", "swiftshiftturkey@gmail.com");
                    MailboxAddress mailboxAddressTo = new MailboxAddress("User", user.Email);

                    mimeMessage.From.Add(mailboxAddressFrom);
                    mimeMessage.To.Add(mailboxAddressTo);

                    var bodyBuilder = new BodyBuilder();
                    bodyBuilder.TextBody = "Kayıt İşlemini Gerçekleştirmek İçin Onay Kodunuz: " + code;
                    mimeMessage.Body = bodyBuilder.ToMessageBody();

                    mimeMessage.Subject = "SwiftShift Onay Kodu";

                    SmtpClient smtpClient = new SmtpClient();
                    smtpClient.Connect("smtp.gmail.com", 587, false);
                    smtpClient.Authenticate("swiftshiftturkey@gmail.com", "hftdjssmjyylkabx");
                    smtpClient.Send(mimeMessage);
                    smtpClient.Disconnect(true);

                    TempData["Mail"] = user.NormalizedEmail;
                    return RedirectToAction("Index", "ConfirmMail");

                    //_logger.LogInformation("User created a new account with password.");

                    //var userId = await _userManager.GetUserIdAsync(user);
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    //if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    //{
                    //    return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    //}
                    //else
                    //{
                    //    await _signInManager.SignInAsync(user, isPersistent: false);
                    //    return LocalRedirect(returnUrl);
                    //}
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
            Image image;
            if (Input.Gender == "Kadın")
            {
                image = Image.FromFile("wwwroot\\images\\kadin.png");
            }
            else
            {
                image = Image.FromFile("wwwroot\\images\\erkek.png");
            }

            using (var ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                return ms.ToArray();
            }
        }

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
                                Coordinates coordinates = await ReCalculateLatLng(lat.Replace('.', ','), longi.Replace('.', ','));
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

    public class Coordinates
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}

