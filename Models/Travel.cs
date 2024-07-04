using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bitirme.Models
{
    public class Travel
    {
        [ForeignKey("Id")]
        public string UserId { get; set; }

        [Key]
        public int TravelId { get; set; }

        [Required]
        [ForeignKey("Country")]
        [DisplayName("Country")]
        public int ArrivalCountryId { get; set; }
        public virtual Country ArrivalCountry { get; set; }

        [Required]
        [ForeignKey("City")]
        [DisplayName("City")]
        public int ArrivalCityId { get; set; }
        public virtual City ArrivalCity { get; set; }

        [Required]
        [ForeignKey("District")]
        [DisplayName("District")]
        public int ArrivalDistrictId { get; set; }
        public virtual District ArrivalDistrict { get; set; }

        [Required]
        [ForeignKey("Country")]
        [DisplayName("Country")]
        public int DepartureCountryId { get; set; }
        public virtual Country DepartureCountry { get; set; }

        [Required]
        [ForeignKey("City")]
        [DisplayName("City")]
        public int DepartureCityId { get; set; }
        public virtual City DepartureCity { get; set; }

        [Required]
        [ForeignKey("District")]
        [DisplayName("District")]
        public int DepartureDistrictId { get; set; }
        public virtual District DepartureDistrict { get; set; }

        public DateTime ArrivalDate { get; set; }
        public DateTime DepartureDate { get; set; }

        public int MaxWeightCarry { get;set; }

        public double pricePerKg { get; set; }

        public int seeProduct { get; set; }

        public int carrySensitiveProduct { get; set; }

        public int isActiveCarrier { get; set; }

    }
}
