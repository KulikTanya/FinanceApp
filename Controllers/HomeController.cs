using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models.ViewModels;

[Authorize]
public class HomeController : Controller
{
    private readonly FinanceTrackerContext _context;

    public HomeController(FinanceTrackerContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
        {
            return Unauthorized();
        }
        
        var accountIds = await _context.Accounts
            .Where(a => a.UserId == userId)
            .Select(a => a.Id)
            .ToListAsync();

        var incomeTypeId = await _context.OperationsType
            .Where(t => t.OperationName == "Доходы")
            .Select(t => t.Id)
            .FirstOrDefaultAsync();

        var expenseTypeId = await _context.OperationsType
            .Where(t => t.OperationName == "Расходы")
            .Select(t => t.Id)
            .FirstOrDefaultAsync();

        var incomes = await _context.OperationsHistory
            .Where(o => o.OperationTypeId == incomeTypeId && accountIds.Contains(o.AccountId))
            .SumAsync(o => (decimal?)o.Amount) ?? 0;

        var expenses = await _context.OperationsHistory
            .Where(o => o.OperationTypeId == expenseTypeId && accountIds.Contains(o.AccountId))
            .SumAsync(o => (decimal?)o.Amount) ?? 0;

        return View(new DashboardViewModel
        {
            TotalIncomes = incomes,
            TotalExpenses = expenses
        });
    }
}