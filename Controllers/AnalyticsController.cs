using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class AnalyticsController : Controller
    {
        private readonly FinanceTrackerContext _context;
        private readonly ILogger<AnalyticsController> _logger;

        public AnalyticsController(
            FinanceTrackerContext context,
            ILogger<AnalyticsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            LoadDropdowns();
            return View(new AnalyticsFilterModel
            {
                StartDate = DateTime.Today.AddMonths(-1),
                EndDate = DateTime.Today
            });
        }

        [HttpGet]
        public IActionResult Analytics(AnalyticsFilterModel filter)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns();
                return View("Index", filter);
            }

            if (filter.StartDate == null || filter.EndDate == null || filter.EndDate < filter.StartDate)
            {
                ModelState.AddModelError("EndDate", "Конечная дата должна быть позже начальной");
                LoadDropdowns();
                return View("Index", filter);
            }

            try
            {
                var analyticsData = GetAnalyticsData(filter);
                ViewBag.AnalyticsData = analyticsData;

                ViewData["ChartLabels"] = analyticsData.Select(x => x.CategoryName).ToList();
                ViewData["ChartData"] = analyticsData.Select(x => x.Amount).ToList();
                ViewData["ChartPercentages"] = analyticsData.Select(x => x.Percentage).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении аналитики");
                ModelState.AddModelError("", "Произошла ошибка при формировании аналитики");
            }

            LoadDropdowns();
            return View("Index", filter);
        }

        private List<AnalyticsItem> GetAnalyticsData(AnalyticsFilterModel filter)
        {
            var startDate = filter.StartDate ?? DateTime.Today.AddMonths(-1);
            var endDate = (filter.EndDate ?? DateTime.Today).Date.AddDays(1).AddSeconds(-1);

            var query = _context.OperationsHistory
                .Include(o => o.OperationCategory)
                .Where(o => o.AccountId == filter.AccountId &&
                           o.OperationTypeId == filter.OperationTypeId &&
                           o.Date >= startDate &&
                           o.Date <= endDate)
                .GroupBy(o => o.OperationCategory.CategoryName)
                .Select(g => new
                {
                    CategoryName = g.Key,
                    TotalAmount = g.Sum(o => o.Amount)
                })
                .AsNoTracking()
                .ToList();

            if (query == null || query.Count == 0)
            {
                return new List<AnalyticsItem>
                {
                    new AnalyticsItem
                    {
                        CategoryName = "Нет данных за выбранный период",
                        Amount = 0,
                        Percentage = 0
                    }
                };
            }

            var totalAmount = query.Sum(x => x.TotalAmount);
            var result = query
                .Select(x => new AnalyticsItem
                {
                    CategoryName = x.CategoryName,
                    Amount = x.TotalAmount,
                    Percentage = totalAmount > 0 ?
                        Math.Round((x.TotalAmount / totalAmount) * 100, 2) : 0
                })
                .OrderByDescending(x => x.Amount)
                .ToList();

            return result;
        }

        private void LoadDropdowns()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            ViewBag.Accounts = _context.Accounts
                .Where(a => a.UserId == userId)
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.AccountNumber
                })
                .ToList();

            ViewBag.OperationTypes = _context.OperationsType
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.OperationName
                })
                .ToList();
        }
    }
}