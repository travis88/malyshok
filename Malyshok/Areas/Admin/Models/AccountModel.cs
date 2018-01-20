using System.ComponentModel.DataAnnotations;

namespace Disly.Areas.Admin.Models
{
    public class LogInModel 
    {
        [Required (ErrorMessage = "Поле «Логин» не должно быть пустым.")]
        public string Login { get; set; }

        [Required (ErrorMessage = "Поле «Пароль» не должно быть пустым.")]
        public string Pass { get; set; }

        public bool RememberMe { get; set; }
    }

    public class RestoreModel 
    {
        [Required (ErrorMessage = "Поле «E-mail» не должно быть пустым.")]
        [RegularExpression (@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Поле «E-mail» заполнено неверно.")]
        public string Email { get; set; }
    }
}