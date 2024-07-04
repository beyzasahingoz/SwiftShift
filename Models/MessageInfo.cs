using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Bitirme.Models
{
    public class MessageInfo 
    {
        [Key]
        public int chatId {  get; set; }
        public string transportUserId { get; set; }

        public string customerUserId { get; set; }

        public int productId { get; set; }

        public int travelId { get; set; }

        public int isCompleted { get; set; }
        public int isPaid { get; set; }

    }
}
