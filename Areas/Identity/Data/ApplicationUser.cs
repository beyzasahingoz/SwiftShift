using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Bitirme.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string Ad { get; set; }
    public string Soyad { get; set; }
    public byte[] ProfilePicture { get; set; }

    public string Point { get; set; }

    public string ProfileDescription { get; set; }

}

