using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class AnalyticsFilterModel
    {
        [Required(ErrorMessage = "Пожалуйста, выберите счёт")]
        [Display(Name = "Счёт")]
        public int? AccountId { get; set; }  

        [Required(ErrorMessage = "Пожалуйста, выберите тип операции")]
        [Display(Name = "Тип операции")]
        public int? OperationTypeId { get; set; } 

        [Required(ErrorMessage = "Пожалуйста, укажите начальную дату")]
        [DataType(DataType.Date)]
        [Display(Name = "Начальная дата")]
        [DateNotInFuture(ErrorMessage = "Начальная дата не может быть в будущем")]
        public DateTime? StartDate { get; set; } = DateTime.Today.AddMonths(-1); 

        [Required(ErrorMessage = "Пожалуйста, укажите конечную дату")]
        [DataType(DataType.Date)]
        [Display(Name = "Конечная дата")]
        [DateNotInFuture(ErrorMessage = "Конечная дата не может быть в будущем")]
        [DateGreaterThan(nameof(StartDate), ErrorMessage = "Конечная дата должна быть позже начальной")]
        public DateTime? EndDate { get; set; } = DateTime.Today;  
    }

    public class DateNotInFutureAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;

            if (value is DateTime dateValue && dateValue.Date > DateTime.Today)
            {
                return new ValidationResult(ErrorMessage ?? "Дата не может быть в будущем");
            }
            return ValidationResult.Success;
        }
    }

    public class DateGreaterThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public DateGreaterThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;

            var currentValue = (DateTime)value;
            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (property == null)
                throw new ArgumentException("Свойство для сравнения не найдено");

            var comparisonValue = property.GetValue(validationContext.ObjectInstance) as DateTime?;

            if (comparisonValue == null) return ValidationResult.Success;

            if (currentValue < comparisonValue.Value)
                return new ValidationResult(ErrorMessage ?? "Конечная дата должна быть позже начальной");

            return ValidationResult.Success;
        }
    }
}