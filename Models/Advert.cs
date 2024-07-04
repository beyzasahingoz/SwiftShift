using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bitirme.Models
{
    public class Advert
    {
        [ForeignKey("Id")]
        public string UserId { get; set; }
        [Key]
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string ProductKg { get; set; }
        public string Note { get; set; }
        public byte[]? ProductImage { get; set; }
    }
}
