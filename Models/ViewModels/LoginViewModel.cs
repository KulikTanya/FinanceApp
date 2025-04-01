using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Логин обязателен")]
        [Display(Name = "Логин")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Пароль обязателен")]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Запомнить меня")]
        public bool RememberMe { get; set; }
    }
}
