using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bitirme.Models
{
    public class MoneyTransaction
    {
        [Key] 
        public int TransactionId { get; set; } 
        public string UserId { get; set; }
        public string IBAN { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public int isPaid { get; set; }
    }
}
