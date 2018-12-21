using System.ComponentModel.DataAnnotations;

namespace SaveWise.DataLayer.Models.Users
{
    public class Register
    {
        [Required(ErrorMessage = "Nazwa użytkownika jest wymagana")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Hasło jest wymagane")]
        public string Password { get; set; }
        
        [Required(ErrorMessage = "Potwierdzenie hasła jest wymagane")]
        public string PasswordConfirm { get; set; }
    }
}