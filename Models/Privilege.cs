namespace WebApplication1.Models
{
    public class Privilege
    {
        public int Id { get; set; }
        public string PrivilegeName { get; set; }

        public ICollection<RolePrivilege> RolePrivileges { get; set; } = new List<RolePrivilege>();
    }
}