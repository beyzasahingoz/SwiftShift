using System.ComponentModel.DataAnnotations.Schema;

namespace Bitirme.Models
{
    public class SignUp
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
    }
}
