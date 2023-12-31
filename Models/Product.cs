using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bitirme.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Display(Name = "Ürün Adı")]
        public string? ProductName { get; set; }

        [Display(Name = "Adres")]
        public string Address { get; set; }

        [Display(Name = "Kilogram")]
        public string ProductKg { get; set; }

        [Display(Name = "Not")]
        public string ProductNote { get; set; }

        public int CityId { get; set; }

        public int CountryId { get; set; }
    }
}
