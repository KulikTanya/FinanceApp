using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class ReportViewModel
    {
        [Required(ErrorMessage = "Выберите счёт")]
        [Display(Name = "Счёт")]
        public int SelectedAccountId { get; set; }

        [Required(ErrorMessage = "Укажите начальную дату")]
        [Display(Name = "Начальная дата")]
        public DateTime StartDate { get; set; } = DateTime.Now.AddMonths(-1);

        [Required(ErrorMessage = "Укажите конечную дату")]
        [Display(Name = "Конечная дата")]
        public DateTime EndDate { get; set; } = DateTime.Now;

        public IEnumerable<Account> Accounts { get; set; }

        public OperationInfo MaxIncome { get; set; }
        public OperationInfo MaxExpense { get; set; }
        public CategoryInfo PopularIncomeCategory { get; set; }
        public CategoryInfo PopularExpenseCategory { get; set; }
        public BalanceDifference Balance { get; set; }
    }

    public class OperationInfo
    {
        public string CategoryName { get; set; }
        public decimal Amount { get; set; }
    }

    public class CategoryInfo
    {
        public string CategoryName { get; set; }
        public int OperationCount { get; set; }
    }

    public class BalanceDifference
    {
        public decimal IncomeTotal { get; set; }
        public decimal ExpenseTotal { get; set; }
        public decimal Difference => IncomeTotal - ExpenseTotal;
    }
}