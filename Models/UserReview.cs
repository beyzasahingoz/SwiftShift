using Bitirme.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bitirme.Models
{
    public class UserReview
    {
        public int Id { get; set; }
        public string ReviewerId { get; set; } 
        public string Review { get; set; }
        public int Rating { get; set; }
        public DateTime ReviewDate { get; set; }
        public ApplicationUser User { get; set; }
        public string ReviewedUserId { get; set; }
    }
}
