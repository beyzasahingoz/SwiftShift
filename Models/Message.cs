using Bitirme.Areas.Identity.Data;
using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bitirme.Models
{
    public class Message
    {
        public int Id { get; set; }
        [Required]
        public string SenderUserName { get; set; }

        [Required]
        public string ReceiverUserName { get; set; }

        [Required]
        public string Text { get; set; }
        public DateTime When { get; set; }

        public string SenderUserID { get; set; }
        public virtual ApplicationUser Sender { get; set; }

        public string ReceiverUserID { get; set; }

        public int isTransporter { get; set; }

        public double Amount { get; set; }

        [ForeignKey("ProductId")]
        public int ProductId { get; set; }

        [ForeignKey("TravelId")]
        public int TravelId { get; set; }
        public Message()
        {
            When = DateTime.Now;
        }
        public bool IsOfferMessage { get; set; }

        public int chatId { get; set; }
    }
}
