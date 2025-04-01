using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Account
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [Display(Name = "Счёт")]
        public string AccountNumber { get; set; }

        public User User { get; set; }
    }
}