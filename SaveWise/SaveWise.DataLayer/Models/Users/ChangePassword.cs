using System.ComponentModel.DataAnnotations;

namespace SaveWise.DataLayer.Models.Users
{
    public class ChangePassword
    {
        [Required(ErrorMessage = "Nazwa użytkownika jest wymagana")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Dotychczasowe hasło nie zostało podane")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Nowe hasło nie może być puste")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Potwierdzenie hasła nie może być puste")]
        public string PasswordConfirm { get; set; }
    }
}