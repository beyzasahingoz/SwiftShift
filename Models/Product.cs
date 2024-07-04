using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bitirme.Models
{
    public class Product
    {
        [ForeignKey("Id")]
        public string UserId { get; set; }

        public string TransporterUserId { get; set; }
        [Key]
        public int ProductId { get; set; }
        [Display(Name = "Ürün Adı")]
        public string? ProductName { get; set; }

        [Display(Name = "Adres")]
        public string Address { get; set; }

        [Display(Name = "Kilogram")]
        public string ProductKg { get; set; }

        [Display(Name = "Not")]
        public string? ProductNote { get; set; }

        [Required]
        [ForeignKey("Country")]
        [DisplayName("Country")]
        public int FromCountryId { get; set; }
        public virtual Country FromCountry { get; set; }

        [Required]
        [ForeignKey("City")]
        [DisplayName("City")]
        public int FromCityId { get; set; }
        public virtual City FromCity { get; set; }

        [Required]
        [ForeignKey("District")]
        [DisplayName("District")]
        public int FromDistrictId { get; set; }
        public virtual District FromDistrict { get; set; }

        [Required]
        [ForeignKey("Country")]
        [DisplayName("Country")]
        public int ToCountryId { get; set; }
        public virtual Country ToCountry { get; set; }

        [Required]
        [ForeignKey("City")]
        [DisplayName("City")]
        public int ToCityId { get; set; }
        public virtual City ToCity { get; set; }

        [Required]
        [ForeignKey("District")]
        [DisplayName("District")]
        public int ToDistrictId { get; set; }
        public virtual District ToDistrict { get; set; }
        public DateTime WhenDate { get; set; }
        public byte[]? ProductImage { get; set; }
        public int path { get; set; }

        public double Amount { get; set; }

        public int isOrder { get; set; }
        public string OrderStatus { get; set; }

        public int isDelivered { get; set; }

        public int isCommentMade { get; set; }  
    }
}
