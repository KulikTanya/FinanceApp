using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ReportController : Controller
    {
        private readonly FinanceTrackerContext _context;
        private readonly ILogger<ReportController> _logger;

        public ReportController(FinanceTrackerContext context, ILogger<ReportController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var userId = GetCurrentUserId();
            var model = new ReportViewModel
            {
                Accounts = GetUserAccounts(userId),
                StartDate = DateTime.Today.AddMonths(-1),
                EndDate = DateTime.Today
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Generate(ReportViewModel model)
        {
            var userId = GetCurrentUserId();

            if (model.EndDate < model.StartDate)
            {
                ModelState.AddModelError("EndDate", "Конечная дата должна быть позже начальной");
                model.Accounts = GetUserAccounts(userId);
                return View("Index", model);
            }

            var adjustedEndDate = model.EndDate.Date.AddDays(1).AddSeconds(-1);
            var adjustedStartDate = model.StartDate.Date.AddDays(1).AddSeconds(-1);

            model.MaxIncome = await GetMaxOperationAsync(model.SelectedAccountId, adjustedStartDate, adjustedEndDate, "Доходы");
            model.MaxExpense = await GetMaxOperationAsync(model.SelectedAccountId, adjustedStartDate, adjustedEndDate, "Расходы");
            model.PopularIncomeCategory = await GetPopularCategoryAsync(model.SelectedAccountId, adjustedStartDate, adjustedEndDate, "Доходы");
            model.PopularExpenseCategory = await GetPopularCategoryAsync(model.SelectedAccountId, adjustedStartDate, adjustedEndDate, "Расходы");
            model.Balance = await GetBalanceDifferenceAsync(model.SelectedAccountId, adjustedStartDate, adjustedEndDate);

            ViewBag.Period = $"{model.StartDate.ToString("dd.MM.yyyy")} - {model.EndDate.ToString("dd.MM.yyyy")}";

            model.Accounts = GetUserAccounts(userId);
            return View("Index", model);
        }

        private async Task<OperationInfo> GetMaxOperationAsync(int accountId, DateTime startDate, DateTime endDate, string operationType)
        {
            return await _context.OperationsHistory
                .Where(oh => oh.AccountId == accountId && oh.Date >= startDate && oh.Date <= endDate && oh.OperationType.OperationName == operationType)
                .OrderByDescending(oh => oh.Amount)
                .Select(oh => new OperationInfo { CategoryName = oh.OperationCategory.CategoryName, Amount = oh.Amount })
                .FirstOrDefaultAsync() ?? new OperationInfo();
        }

        private async Task<CategoryInfo> GetPopularCategoryAsync(int accountId, DateTime startDate, DateTime endDate, string operationType)
        {
            return await _context.OperationsHistory
                .Where(oh => oh.AccountId == accountId && oh.Date >= startDate && oh.Date <= endDate && oh.OperationType.OperationName == operationType)
                .GroupBy(oh => oh.OperationCategory.CategoryName)
                .OrderByDescending(g => g.Count())
                .Select(g => new CategoryInfo { CategoryName = g.Key, OperationCount = g.Count() })
                .FirstOrDefaultAsync() ?? new CategoryInfo();
        }

        private async Task<BalanceDifference> GetBalanceDifferenceAsync(int accountId, DateTime startDate, DateTime endDate)
        {
            var incomeTotal = await _context.OperationsHistory
                .Where(oh => oh.AccountId == accountId && oh.Date >= startDate && oh.Date <= endDate && oh.OperationType.OperationName == "Доходы")
                .SumAsync(oh => (decimal?)oh.Amount) ?? 0;

            var expenseTotal = await _context.OperationsHistory
                .Where(oh => oh.AccountId == accountId && oh.Date >= startDate && oh.Date <= endDate && oh.OperationType.OperationName == "Расходы")
                .SumAsync(oh => (decimal?)oh.Amount) ?? 0;

            return new BalanceDifference { IncomeTotal = incomeTotal, ExpenseTotal = expenseTotal };
        }

        private IEnumerable<Account> GetUserAccounts(int userId)
        {
            return _context.Accounts.Where(a => a.UserId == userId).OrderBy(a => a.AccountNumber).ToList();
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
    }
}
