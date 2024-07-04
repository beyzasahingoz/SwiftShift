using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bitirme.Models
{
    public class CascadingModel
    {
        public CascadingModel()
        {
            this.Countries = new List<SelectListItem>();
            this.Cities = new List<SelectListItem>();
            this.District = new List<SelectListItem>();
        }

        public List<SelectListItem> Countries { get; set; }
        public List<SelectListItem> Cities { get; set; }
        public List<SelectListItem> District { get; set; }

        public int CountryId { get; set; }

        public int CityId { get; set; }

        public int DistrictId { get; set; }
    }
}
