using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Bitirme.Models;
using Microsoft.AspNetCore.Identity;

namespace Bitirme.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string Ad { get; set; }
    public string Soyad { get; set; }
    public byte[]? ProfilePicture { get; set; }

    public string? Point { get; set; }

    public string? ProfileDescription { get; set; }

    [Required]
    [ForeignKey("Country")]
    [DisplayName("Country")]
    public int CountryId { get; set; }
    public virtual Country Country { get; set; }

    [Required]
    [ForeignKey("City")]
    [DisplayName("City")]
    public int CityId { get; set; }
    public virtual City City { get; set; }
}

