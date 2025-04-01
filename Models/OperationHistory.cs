using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class OperationHistory
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле 'Дата' обязательно для заполнения")]
        [Display(Name = "Дата")]
        public DateTime Date { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Выберите один из пунктов списка")]
        [Display(Name = "Счёт")]
        public int AccountId { get; set; }

        [Required(ErrorMessage = "Выберите один из пунктов списка")]
        [Display(Name = "Категория операции")]
        public int OperationCategoryId { get; set; }

        [Required(ErrorMessage = "Поле 'Сумма' обязательно для заполнения")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Сумма должна быть больше нуля")]
        [Display(Name = "Сумма")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Выберите один из пунктов списка")]
        [Display(Name = "Тип операции")]
        public int OperationTypeId { get; set; }

        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }

        [ForeignKey("OperationCategoryId")]
        public virtual OperationCategory OperationCategory { get; set; }

        [ForeignKey("OperationTypeId")]
        public virtual OperationType OperationType { get; set; }
    }
}