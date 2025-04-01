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

        var incomes = await _context.OperationsHistory
            .Where(o => o.OperationTypeId == 1 &&
                        _context.Accounts.Any(a => a.Id == o.AccountId && a.UserId == userId))
            .SumAsync(o => (decimal?)o.Amount) ?? 0;

        var expenses = await _context.OperationsHistory
            .Where(o => o.OperationTypeId == 2 &&
                        _context.Accounts.Any(a => a.Id == o.AccountId && a.UserId == userId))
            .SumAsync(o => (decimal?)o.Amount) ?? 0;

        var model = new DashboardViewModel
        {
            TotalIncomes = incomes,
            TotalExpenses = expenses
        };

        return View(model);
    }
}
