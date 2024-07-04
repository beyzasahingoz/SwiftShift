using Bitirme.Areas.Identity.Data;
using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace Bitirme.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        [Required]
        public string SenderUserName { get; set; }

        [Required]
        public string ReceiverUserName { get; set; }

        [Required]
        public string CommentText { get; set; }
        public DateTime When { get; set; }

        public string SenderUserID { get; set; }

        public string ReceiverUserID { get; set; }

        public double Rating { get; set; }

        public int ProductId { get; set; }

        public string senderNameSurname { get; set; }

        public byte[] senderProfilePicture { get; set; }

       
    }
}
