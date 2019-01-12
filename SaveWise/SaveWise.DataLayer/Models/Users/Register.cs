using System.ComponentModel.DataAnnotations;

namespace SaveWise.DataLayer.Models.Users
{
    public class Register
    {
        [Required(ErrorMessage = "Nazwa użytkownika jest wymagana")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Hasło jest wymagane")]
        [RegularExpression(@"(?=^.{8,}$)(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?!.*\s)[0-9a-zA-Z!@#$%^&*()]*$",
            ErrorMessage = "Hasło musi posiadać jedną małą i wielką literę oraz cyfrę")]
        [MinLength(8, ErrorMessage = "Hasło musi mieć minimum 8 znaków")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Potwierdzenie hasła jest wymagane")]
        public string PasswordConfirm { get; set; }
    }
}