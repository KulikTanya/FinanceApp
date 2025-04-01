using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.ViewModel
{
    public class RegisterViewModel
    {

        [Required(ErrorMessage = "Логин обязателен")]
        [Display(Name = "Логин")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Логин должен быть от 3 до 50 символов")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Пароль обязателен")]
        [Display(Name = "Пароль")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль должен быть от 6 до 100 символов")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Повторите пароль")]
        [Required(ErrorMessage = "Повторить пароль обязательно")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Имя обязательно")]
        [StringLength(100, ErrorMessage = "Имя не должно превышать 100 символов")]
        [Display(Name = "Имя")]
        public string Name { get; set; }
    }
}
