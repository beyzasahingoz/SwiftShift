using Bitirme.Areas.Identity.Data;
using Microsoft.Build.Framework;

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
        public virtual ApplicationUser Receiver { get; set; }

        public Message()
        {
            When = DateTime.Now;
        }
    }
}
