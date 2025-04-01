using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class OperationHistoryController : Controller
    {
        private readonly FinanceTrackerContext _context;
        private readonly ILogger<OperationHistoryController> _logger;

        public OperationHistoryController(
            FinanceTrackerContext context,
            ILogger<OperationHistoryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View(new OperationHistory { Date = DateTime.Now });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OperationHistory operation)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                                          
            try
            {
                await _context.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT INTO dbo.OperationsHistory (date, accountId, operationCategoryId, amount, operationTypeId) VALUES ({operation.Date}, {operation.AccountId}, {operation.OperationCategoryId}, {operation.Amount}, {operation.OperationTypeId})");

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении операции");
                ModelState.AddModelError("", "Ошибка при сохранении операции");
                await LoadDropdowns();
                return View(operation);
            }
        }

        private async Task LoadDropdowns()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            ViewBag.Accounts = await _context.Accounts
                .Where(a => a.UserId == userId)
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.AccountNumber
                })
                .ToListAsync();

            ViewBag.OperationTypes = await _context.OperationsType
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.OperationName
                })
                .ToListAsync();

            ViewBag.OperationCategories = await _context.OperationsCategories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CategoryName
                })
                .ToListAsync();
        }
    }
}