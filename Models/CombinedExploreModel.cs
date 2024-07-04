using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bitirme.Models
{
    public class CombinedExploreModel
    {
        public List<Product> Products { get; set; }
        public List<Travel> Travels { get; set; }
    }
}
