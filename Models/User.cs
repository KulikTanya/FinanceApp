using System.Collections.Generic;
using System.Data;

namespace WebApplication1.Models
{
    public class User
    {
        public User()
        {
            Accounts = new HashSet<Account>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }
        public ICollection<Account> Accounts { get; set; }
    }
}