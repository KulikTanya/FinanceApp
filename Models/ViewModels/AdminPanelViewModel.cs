namespace WebApplication1.Models.ViewModels
{
    public class AdminPanelViewModel
    {
        public List<User> Users { get; set; }
        public List<Role> Roles { get; set; }
        public List<Privilege> Privileges { get; set; }
        public List<RolePrivilege> RolePrivileges { get; set; }

        public Dictionary<int, List<int>> RolePrivilegeMap =>
            RolePrivileges.GroupBy(rp => rp.RoleId)
                          .ToDictionary(g => g.Key, g => g.Select(rp => rp.PrivilegeId).ToList());
    }
}
