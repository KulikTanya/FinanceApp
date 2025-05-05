using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class AccountManagementController : Controller
    {
        private readonly FinanceTrackerContext _context;
        private readonly ILogger<AccountManagementController> _logger; 

        public AccountManagementController(
            FinanceTrackerContext context,
            ILogger<AccountManagementController> logger) 
        {
            _context = context;
            _logger = logger; 
        }

        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var accounts = await _context.Accounts
                .Where(a => a.UserId == userId)
                .ToListAsync();

            return View(accounts);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccountNumber")] Account account)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (_context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
            {
                account.UserId = userId;
                _context.Accounts.Add(account);
            }
            else
            {
                await _context.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT INTO dbo.Accounts (UserId, AccountNumber) VALUES ({userId}, {account.AccountNumber})");
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var account = await _context.Accounts.FindAsync(id);
            if (account == null) return NotFound();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (account.UserId != userId) return Forbid();

            return View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AccountNumber")] Account account)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE dbo.Accounts SET AccountNumber = {account.AccountNumber} WHERE Id = {id} AND UserId = {userId}");

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null) return NotFound();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (account.UserId != userId) return Forbid();

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}