using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;

namespace WebApplication1.Controllers
{
    public class AdminController : Controller
    {
        private readonly FinanceTrackerContext _context;

        public AdminController(FinanceTrackerContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var currentUser = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == currentUserId);            

            var model = new AdminPanelViewModel
            {
                Users = await _context.Users.Include(u => u.Role).ToListAsync(),
                Roles = await _context.Roles.ToListAsync(),
                Privileges = await _context.Privileges.ToListAsync(),
                RolePrivileges = await _context.RolePrivileges.ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUserRole(int userId, int roleId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            user.RoleId = roleId;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRolePrivileges(int roleId, List<int> privilegeIds)
        {
            var existingPrivileges = _context.RolePrivileges.Where(rp => rp.RoleId == roleId);
            _context.RolePrivileges.RemoveRange(existingPrivileges);

            foreach (var privilegeId in privilegeIds)
            {
                _context.RolePrivileges.Add(new RolePrivilege
                {
                    RoleId = roleId,
                    PrivilegeId = privilegeId
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
